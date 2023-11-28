using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace RabbetGameEngine.Source.Rendering.RenderObjects;

public class TextureAtlasPow2
{
    /// <summary>
    /// A map for retreiving a sprites min and max UV coordinates in the atlas texture based on name.
    /// </summary>
    public Dictionary<string, Vector4> spriteMap
    {
        get;
        private set;
    }

    /// <summary>
    /// Texture containing all sprite textures packed tightly with no rotations or padding.
    /// </summary>
    public Texture atlasTexture
    {
        get;
        private set;
    }

    public int texAtlasSize
    {
        get;
        private set;
    }

    public string spriteFolderDir
    {
        get;
        private set;
    }

    public bool creationSuccess
    {
        get;
        private set;
    }

    private int searchedXPxCount = 0;
    private int searchedYPxCount = 0;

    public TextureAtlasPow2(string spriteDir)
    {
        creationSuccess = false;
        spriteFolderDir = spriteDir;
        spriteMap = new Dictionary<string, Vector4>();
        List<Sprite> loadedSprites = new List<Sprite>();
        List<Sprite> squarePow2Sprites = new List<Sprite>();
        loadAllSpritesRecursive(spriteDir, loadedSprites);

        if (loadedSprites == null || loadedSprites.Count == 0)
        {
            Application.warn("TextureAtlasPow2 could not find any images at directory: " + spriteDir);
            return;
        }

        //load sprites, only accepting square sprites with power of 2 dimentions. Sort by size.
        foreach (Sprite s in loadedSprites)
        {
            int sW = s.Width;
            int sH = s.Height;
            if ((sW & sW - 1) != 0 || sW != sH)
            {
                if ((sW & sW - 1) != 0)
                    Application.warn("TextureAtlasPow2 could not accept sprite \"" + s.name + "\". Sprite does not have power of 2 size. Width: " + sW + ", Height: " + sH);
                else if (sW != sH)
                    Application.warn("TextureAtlasPow2 could not accept sprite \"" + s.name + "\". Sprite is not square. Width: " + sW + ", Height: " + sH);
                s.Dispose();
                continue;
            }
        }
        squarePow2Sprites.Sort(new TextureSizeSorter());

        //calculate minimum texture atlas dimensions
        int texSizeTotal = 0;
        foreach (Sprite s in squarePow2Sprites)
            texSizeTotal += s.Width * s.Height;

        int texSizeTotalPow2 = roundUpToPow2(texSizeTotal);
        texAtlasSize = MathUtil.ceilToInt(MathF.Sqrt(texSizeTotalPow2));

        float packingStartTime = TicksAndFrames.getRealTimeMills();
        //pack sprite pixels into atlas, recording their UV's in the map by name.
        packSpritesIntoAtlas(squarePow2Sprites);
        float packingEndTime = TicksAndFrames.getRealTimeMills();
        float timeTaken = packingEndTime - packingStartTime;
        Application.infoPrint("Texture atlas " + spriteDir + " took " + timeTaken + " seconds to load. Loaded " + spriteMap.Values.Count + " sprites out of " + squarePow2Sprites.Count);
        creationSuccess = true;
    }

    private void packSpritesIntoAtlas(List<Sprite> sprites)
    {
        Bitmap atlasMap = new Bitmap(texAtlasSize, texAtlasSize);
        Sprite currentRowBiggestSprite = sprites[0];
        Sprite prevSprite = sprites[0];
        int rowFilledX = 0;
        int filledY = 0;
        for (int i = 0; i < sprites.Count; i++)
        {
            Sprite s = sprites[i];
            if (s.Width + rowFilledX >= texAtlasSize)
            {
                filledY += prevSprite.Height;
                rowFilledX = currentRowBiggestSprite.Width;
                if (filledY >= currentRowBiggestSprite.Height)
                {
                    currentRowBiggestSprite = s;
                    rowFilledX = 0;
                }
            }
            packSprite(atlasMap, s, rowFilledX, filledY);
            rowFilledX += s.Width;
            prevSprite = s;
        }

        atlasTexture = new Texture(atlasMap);
        atlasMap.Dispose();
    }

    private void packSprite(Bitmap atlasMap, Sprite s, int xOffset, int yOffset)
    {
        for (int x = 0; x < s.Width; x++)
            for (int y = 0; y < s.Height; y++)
                atlasMap.SetPixel(xOffset + x, yOffset + y, s.pixels.GetPixel(x, y));
        //Array.Copy(sPixels, y * s.Width, bitMap, (yOffset + y) * texAtlasSize + xOffset, s.Width);
        onSpriteFullyRead(s, xOffset + (s.Width - 1), yOffset + (s.Height - 1));
    }

    private void onSpriteFullyRead(Sprite s, int finalX, int finalY)
    {
        Vector4 resultMinMaxUV = new Vector4(0, 0, 0, 0);
        resultMinMaxUV.X = (finalX - s.Width + 1.0F) / texAtlasSize;
        resultMinMaxUV.Y = (finalY - s.Height + 1.0F) / texAtlasSize;
        resultMinMaxUV.Z = (float)(finalX + 1) / texAtlasSize;
        resultMinMaxUV.W = (float)(finalY + 1) / texAtlasSize;
        spriteMap[s.name] = resultMinMaxUV;
        searchedXPxCount += s.Width;
        searchedYPxCount += s.Height;
        s.Dispose();
    }

    private int roundUpToPow2(int i)
    {
        i--;
        i |= i >> 1;
        i |= i >> 2;
        i |= i >> 4;
        i |= i >> 8;
        i |= i >> 16;
        i++;
        return i;
    }

    private void loadAllSpritesRecursive(string path, List<Sprite> sprites)
    {
        try
        {
            string[] allFiles = Directory.GetFiles(path);
            string[] allDirectories = Directory.GetDirectories(path);
            foreach (string file in allFiles)
            {
                if (file.Contains(".png"))
                {
                    sprites.Add(new Sprite(file));
                }
            }

            foreach (string dir in allDirectories)
            {
                loadAllSpritesRecursive(dir, sprites);
            }
        }
        catch (Exception e)
        {
            Application.error(e.Message);
        }
    }

    private class Sprite : IDisposable
    {
        public Bitmap pixels;
        public string name;

        public int Width
        {
            get
            {
                return pixels.Width;
            }
        }

        public int Height
        {
            get
            {
                return pixels.Height;
            }
        }

        public Sprite(string path)
        {
            this.name = Path.GetFileName(path).Replace(".png", "").ToLower();//removes directory
            this.pixels = new Bitmap(path);
        }

        public void Dispose()
        {
            if (pixels != null)
                pixels.Dispose();
        }
    }

    private class TextureSizeSorter : IComparer<Sprite>
    {
        public int Compare(Sprite x, Sprite y)
        {
            if (x.Width < y.Width)
                return 1;
            if (x.Width == y.Width)
                return 0;

            return -1;
        }
    }
}