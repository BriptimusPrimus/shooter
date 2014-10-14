using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.IO;
using System.Threading;
using Game2DKit.SoundMangement;

namespace Game2DKit
{
    public class ContentManager
    {
        public string ImagesDirectory { get; set; }
        public string SoundDirectory { get; set; }

        public Texture2D LoadTexture(string file)
        {
            string fileDir = this.ImagesDirectory + "/" + file;
            if (String.IsNullOrEmpty(fileDir))
            {
                throw new ArgumentException(fileDir);            
            }

            if(!File.Exists(fileDir))
            {
                throw new ArgumentException(fileDir);
            }

            Bitmap bitmap = new Bitmap(fileDir);
            //bitmap.MakeTransparent(Color.Magenta); White

            int tex;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            //result Texture2D object
            Texture2D result = new Texture2D(tex, file, bitmap.Width, bitmap.Height);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return result;
        }       

        public SoundEffect LoadSoundEffect(string file)
        {
            string fileDir = this.SoundDirectory + "/" + file;
            if (String.IsNullOrEmpty(fileDir))
            {
                throw new ArgumentException(fileDir);
            }

            if (!File.Exists(fileDir))
            {
                throw new ArgumentException(fileDir);
            }

            return new SoundEffect(fileDir);
        }

        public Song LoadSong(string file, int duration)
        {
            string fileDir = this.SoundDirectory + "/" + file;
            if (String.IsNullOrEmpty(fileDir))
            {
                throw new ArgumentException(fileDir);
            }

            if (!File.Exists(fileDir))
            {
                throw new ArgumentException(fileDir);

                // Safety pig has arrived!
                //                                 _
                //    _._ _..._ .-',     _.._(`))
                //   '-. `     '  /-._.-'    ',/
                //      )         \            '.
                //     / _    _    |             \
                //    |  a    a    /              |
                //    \   .-.                     ;  
                //     '-('' ).-'       ,'       ;
                //        '-;           |      .'
                //           \           \    /
                //           | 7  .__  _.-\   \
                //           | |  |  ``/  /`  /
                //          /,_|  |   /,_/   /
                //             /,_/      '`-'
                //  
            }

            SoundEffect sound = new SoundEffect(fileDir);

            return new Song(sound, duration);
        }


    }
}
