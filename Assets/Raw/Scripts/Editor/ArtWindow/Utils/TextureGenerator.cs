using UnityEngine;

namespace RuntimeArtWay {
    public static class TextureGenerator {

        public static Texture2D GenerateBox(int width, int height, Color background){
            return GenerateBox(width, height, background, 1, Color.black);
        }

        public static Texture2D GenerateBox(int width, int height, Color background, int borderSize, Color border){
            var result = new Texture2D(width, height);

            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++){
                int x = i % width;
                int y = i / height;

                if (x > borderSize || x < width - borderSize
                    || y > borderSize || y < height - borderSize){
                        pixels[i] = background;
                }
                else {
                    pixels[i] = border;
                }
            }
            result.SetPixels(pixels);
            result.Apply();

            return result;
        }
    }
}