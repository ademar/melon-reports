using System;
using System.IO;
using Melon.Pdf.Objects;

namespace Melon.Pdf.Imaging
{
	public class GifImage : AbstractImage, IDisposable 
	{
		protected enum gif_states {
			gif_gather,
			gif_init,                   /*1*/
			gif_type,
			gif_version,
			gif_global_header,
			gif_global_colormap,
			gif_image_start,            /*6*/
			gif_image_header,
			gif_image_colormap,
			gif_image_body,
			gif_lzw_start,
			gif_lzw,                    /*11*/
			gif_sub_block,
			gif_extension,
			gif_control_extension,
			gif_consume_block,
			gif_skip_block,
			gif_done,                   /*17*/
			gif_oom,
			gif_error,
			gif_comment_extension,
			gif_application_extension,
			gif_netscape_extension_block,
			gif_consume_netscape_extension,
			gif_consume_comment,
			gif_delay,
			gif_wait_for_buffer_full,
			gif_stop_animating 
		};

		MemoryStream ms;

		public GifImage(string href):base(href)
		{}

		protected override void LoadImage()
		{
			m_bitsPerPixel = 8 ;
			m_isTransparent = false ;
			m_colorSpace = new ColorSpace();
			ColorSpace.AddDevice(ColorDevice.Indexed);
			ColorSpace.AddDevice(ColorDevice.DeviceRGB);
			
			//add the LZW filter
			m_filter = new LZWFilter();//migth need some parameters! '/EarlyChange' for instance

			var ImageHolder = new FileStream(m_href,FileMode.Open,FileAccess.Read,FileShare.Read,8192);
			var len = (int)ImageHolder.Length;
			var buf =  new byte[len+1];//the +1 is a dirty hack , don't like it
			ImageHolder.Read(buf,0,len);
			
			var state = gif_states.gif_init;
			var colorTableSize = 0 ;
			var p  = 0 ;
			
			while (true)
			{
				switch(state)
				{
					case gif_states.gif_init:
					
						if (buf[0]!=0x47 || buf[1]!=0x49 || buf[2]!=0x46 )
							throw new ImageFormatException("Invalid GIF format : No a GIF file.");
					
						/// p = 6 ; // version ~ uninsteresting
							
						m_width  = toShortInverted(buf[6],buf[7]);
						m_height = toShortInverted(buf[8],buf[9]);

						byte packed_field = buf[10];

						colorTableSize = 2 << (packed_field & 0x07);

						//byte 11: Background Color Index 
						//byte 12: Pixel Aspect Ratio 

						p = 13 ;

						state = (packed_field & 0x80) == 0 ? gif_states.gif_image_start : gif_states.gif_global_colormap;
						break;
						
					case gif_states.gif_global_colormap :
						
						var ct =  new ColorTable(colorTableSize);

						for(int i=0 ; i< colorTableSize; i++)
						{
							ct.AddItem(buf[p++]);//red
							ct.AddItem(buf[p++]);//green
							ct.AddItem(buf[p++]);//blue
						}

						ColorSpace.AddColorTable(ct);
						state = gif_states.gif_image_start;
						break;

					case gif_states.gif_image_start:
						int first_code = buf[p++];
						if (first_code==0x3b) //terminator ';'
						{
							state = gif_states.gif_done;
							break;
						}
						if (first_code==0x21) //extension '!'
						{
							state = gif_states.gif_extension;
							break;
						}
						if (first_code!=0x2c) //invalid format , should be ','
							throw new ImageFormatException("Invalid GIF format : image separator code (0x2C) not found.");
						
						state = gif_states.gif_image_header;

						break;

					case gif_states.gif_extension:
						int ext_code = buf[p++];
						int jump_len = buf[p++];
						state = gif_states.gif_skip_block ;

					switch(ext_code)
					{
						case 0xf9:
							state = gif_states.gif_control_extension;
							break;
						case 0x01:
							//just ignoring plain text extension
							break;
						case 0xff:
							state = gif_states.gif_application_extension;
							break;
						case 0xfe:
							state = gif_states.gif_consume_comment;
							break;
					}
						if (jump_len>0) 
						{
							p = p + jump_len ;
						}
						else 
							state = gif_states.gif_image_start;

						break;

					case gif_states.gif_consume_block:
						if (buf[p]==0)
						{
							state = gif_states.gif_image_start;
							p++ ; 
						}
						else
						{
							p = p + buf[p];
							state = gif_states.gif_skip_block;
						}
						break;
					case gif_states.gif_skip_block:
						
						state = gif_states.gif_image_start;
						break;
					case gif_states.gif_control_extension:
						//    here i should check for transparency 
						state = gif_states.gif_consume_block;
						break;
					case gif_states.gif_comment_extension:
						if(buf[p]>0)
						{
							p=p+buf[p];
							state = gif_states.gif_consume_comment;
						}
						else 
							state = gif_states.gif_image_start;
						break;
					case gif_states.gif_consume_comment:
						state = gif_states.gif_comment_extension ;
						break;
					case gif_states.gif_application_extension:
						state = gif_states.gif_consume_block;
						break;
					case gif_states.gif_image_header:
						p = p + 8 ;

						packed_field = buf[p++];

						int localColorTableFlag = (packed_field & 0x80); //bit 1
						int interlaceFlag = (packed_field & 0x40); //bit 2
						int sizeLocalColorTable = 2 << (packed_field & 0x07); //last 3 bits

						if (localColorTableFlag>0)
						{
							throw new ImageFormatException("Invalid GIF format : Local Color Tables not supported.");
						}
						if (interlaceFlag>0)
						{
							throw new ImageFormatException("Invalid GIF format : Interlaced GIF's not supported.");
						}

						//i'm assuming there's no local color table 
						state = gif_states.gif_lzw_start;
						break;
					case gif_states.gif_lzw_start:
						p++;
						state = gif_states.gif_sub_block;
						break;
					case gif_states.gif_sub_block:
						if (buf[p]!=0 )
						{	//read ahead
							proc_sblock(buf,buf[p],p+1);
							ms.Flush();
							m_bitmaps = ms.ToArray();
							return;
						}
				
						break;
					case gif_states.gif_done:
						return;
				
				}
			}
						
		}

		
		private void proc_sblock(byte[] buf,int l,int p)
		{
			int codelength = 9;
			int tablelength = 257;
			int bitstowrite = 0;
			int bitsdone = 0;
			int bitsleft = 23;
			int bytesread = 0;
			int byteswritten = 0;
			int size = l ;

			ms = new MemoryStream();

			// if possible, we read the first 24 bits of data
			size--; bytesread++; int bitsread = buf[p++];
			if (size > 0) 
			{
				size--; bytesread++; bitsread += (buf[p++] << 8);
				if (size > 0) 
				{
					size--; bytesread++; bitsread += (buf[p++] << 16);
				}
			}

			while (bytesread > byteswritten) 
			{
				tablelength++;
				// we extract a code with length=codelength
				int code = (bitsread >> bitsdone) & ((1 << codelength) - 1);
				// we delete the bytesdone in bitsread and append the next byte(s)
				int bytesdone = (bitsdone + codelength) / 8;
				bitsdone = (bitsdone + codelength) % 8;
				while (bytesdone > 0) 
				{
					bytesdone--;
					bitsread = (bitsread >> 8);
					if (size > 0) 
					{
						size--; bytesread++; bitsread += (buf[p++] << 16);
					}
					else 
					{
						size = buf[p++];
						if (size > 0) 
						{
							size--; bytesread++; bitsread += (buf[p++] << 16);
						}
					}
				}

				// we package all the bits that are done into bytes and write them to the stream
				bitstowrite += (code << (bitsleft - codelength + 1));
				bitsleft -= codelength;

				while (bitsleft < 16) 
				{
					var kk = (byte)(bitstowrite >> 16);
					ms.WriteByte(kk);
					byteswritten++;
					bitstowrite = (bitstowrite & 0xFFFF) << 8;
					bitsleft += 8;
				}
				if (code == 256) 
				{
					codelength = 9;
					tablelength = 257;
				}
				if (code == 257) 
				{
					break;
				}
				if (tablelength == (1 << codelength)) 
				{
					codelength++;
				}
			}
			if (bytesread - byteswritten > 2) 
			{
				throw new ImageFormatException("Invalid GIF format : unexpected end of data block.");
			}

		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				ms.Close();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

	}
}
