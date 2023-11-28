using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

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
        List<Sprite> squarePow2Sprites = new List<Sprite>();
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(spriteDir);

        if (loadedSprites == null || loadedSprites.Length == 0)
        {
            Debug.LogWarning("TextureAtlasPow2 could not find any sprites at directory: " + spriteDir);
            return;
        }

        //load sprites, only accepting square sprites with power of 2 dimentions. Sort by size.
        foreach (Sprite s in loadedSprites)
        {
            int sW = s.texture.width;
            int sH = s.texture.height;
            if ((sW & sW - 1) != 0 || sW != sH)
            {
                if ((sW & sW - 1) != 0)
                    Debug.LogWarning("TextureAtlasPow2 could not accept sprite \"" + s.name + "\". Sprite does not have power of 2 size. Width: " + sW + ", Height: " + sH);
                else if (sW != sH)
                    Debug.LogWarning("TextureAtlasPow2 could not accept sprite \"" + s.name + "\". Sprite is not square. Width: " + sW + ", Height: " + sH);
                continue;
            }
            squarePow2Sprites.Add(s);
        }
        squarePow2Sprites.Sort(new TextureSizeSorter());

        //calculate minimum texture atlas dimensions
        int texSizeTotal = 0;
        foreach (Sprite s in squarePow2Sprites)
            texSizeTotal += s.texture.width * s.texture.height;

        int texSizeTotalPow2 = roundUpToPow2(texSizeTotal);
        texAtlasSize = Mathf.CeilToInt(Mathf.Sqrt(texSizeTotalPow2));

        float packingStartTime = Time.realtimeSinceStartup;
        //pack sprite pixels into atlas, recording their UV's in the map by name.
        packSpritesIntoAtlas(squarePow2Sprites);
        float packingEndTime = Time.realtimeSinceStartup;
        float timeTaken = packingEndTime - packingStartTime;
        Debug.Log("Texture atlas " + spriteDir + " took " + timeTaken + " seconds to load. Loaded " + spriteMap.Values.Count + " sprites out of " + squarePow2Sprites.Count);
        creationSuccess = true;
    }

    private void packSpritesIntoAtlas(List<Sprite> sprites)
    {
        Color[] bitMap = new Color[texAtlasSize * texAtlasSize];

        Sprite currentRowBiggestSprite = sprites[0];
        Sprite prevSprite = sprites[0];
        int rowFilledX = 0;
        int filledY = 0;
        for (int i = 0; i < sprites.Count; i++)
        {
            Sprite s = sprites[i];
            if (s.texture.width + rowFilledX >= texAtlasSize)
            {
                filledY += prevSprite.texture.height;
                rowFilledX = currentRowBiggestSprite.texture.width;
                if (filledY >= currentRowBiggestSprite.texture.height)
                {
                    currentRowBiggestSprite = s;
                    rowFilledX = 0;
                }
            }
            packSprite(bitMap, s, rowFilledX, filledY);
            rowFilledX += s.texture.width;
            prevSprite = s;
        }

        atlasTexture = new Texture(texAtlasSize, texAtlasSize);
    }

    private void packSprite(Color[] bitMap, Sprite s, int xOffset, int yOffset)
    {
        Color[] sPixels = s.texture.GetPixels();

        for (int y = 0; y < s.texture.height; y++)
        {
            Array.Copy(sPixels, y * s.texture.width, bitMap, (yOffset + y) * texAtlasSize + xOffset, s.texture.width);
        }
        onSpriteFullyRead(s, xOffset + (s.texture.width - 1), yOffset + (s.texture.height - 1));
    }

    private void onSpriteFullyRead(Sprite s, int finalX, int finalY)
    {
        Vector4 resultMinMaxUV = new Vector4(0, 0, 0, 0);
        resultMinMaxUV.X = (finalX - s.texture.width + 1.0F) / texAtlasSize;
        resultMinMaxUV.Y = (finalY - s.texture.height + 1.0F) / texAtlasSize;
        resultMinMaxUV.Z = (float)(finalX + 1) / texAtlasSize;
        resultMinMaxUV.W = (float)(finalY + 1) / texAtlasSize;
        spriteMap[s.name] = resultMinMaxUV;
        searchedXPxCount += s.texture.width;
        searchedYPxCount += s.texture.height;
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

    private class TextureSizeSorter : IComparer<Sprite>
    {
        public int Compare(Sprite x, Sprite y)
        {
            if (x.width < y.width)
                return 1;
            if (x.width == y.width)
                return 0;

            return -1;
        }
    }
}