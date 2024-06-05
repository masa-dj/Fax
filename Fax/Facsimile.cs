﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fax
{
    internal class Facsimile
    {
        //4 kodne tabele: terminating codes:bele i crne, i makeup codes:bele i crne za RLE iznad 63
        //koje su predefinisane

        private static readonly Dictionary<int, string> WhiteTerminatingCodes = new Dictionary<int, string>
        {
            { 0, "00110101" }, { 1, "000111" }, { 2, "0111" }, { 3, "1000" },
            { 4, "1011" }, { 5, "1100" }, { 6, "1110" }, { 7, "1111" },
            { 8, "10011" }, { 9, "10100" }, { 10, "00111" }, { 11, "01000" },
            { 12, "001000" }, { 13, "000011" }, { 14, "110100" }, { 15, "110101" },
            { 16, "101010" }, { 17, "101011" }, { 18, "0100111" }, { 19, "0001100" },
            { 20, "0001000" }, { 21, "0010111" }, { 22, "0000011" }, { 23, "0000100" },
            { 24, "0101000" }, { 25, "0101011" }, { 26, "0010011" }, { 27, "0100100" },
            { 28, "0011000" }, { 29, "00000010" }, { 30, "00000011" }, { 31, "00011010" },
            { 32, "00011011" }, { 33, "00010010" }, { 34, "00010011" }, { 35, "00010100" },
            { 36, "00010101" }, { 37, "00010110" }, { 38, "00010111" }, { 39, "00101000" },
            { 40, "00101001" }, { 41, "00101010" }, { 42, "00101011" }, { 43, "00101100" },
            { 44, "00101101" }, { 45, "00000100" }, { 46, "00000101" }, { 47, "00001010" },
            { 48, "00001011" }, { 49, "01010010" }, { 50, "01010011" }, { 51, "01010100" },
            { 52, "01010101" }, { 53, "00100100" }, { 54, "00100101" }, { 55, "01011000" },
            { 56, "01011001" }, { 57, "01011010" }, { 58, "01011011" }, { 59, "01001010" },
            { 60, "01001011" }, { 61, "00110010" }, { 62, "00110011" }, { 63, "00110100" }
        };
        private static readonly Dictionary<int, string> WhiteMakeupCodes = new Dictionary<int, string>
        {
            { 64, "11011" }, { 128, "10010" }, { 192, "010111" }, { 256, "0110111" },
            { 320, "00110110" }, { 384, "00110111" }, { 448, "01100100" }, { 512, "01100101" },
            { 576, "01101000" }, { 640, "01100111" }, { 704, "011001100" }, { 768, "011001101" },
            { 832, "011010010" }, { 896, "011010011" }, { 960, "011010100" }, { 1024, "011010101" },
            { 1088, "011010110" }, { 1152, "011010111" }, { 1216, "011011000" }, { 1280, "011011001" },
            { 1344, "011011010" }, { 1408, "011011011" }, { 1472, "010011000" }, { 1536, "010011001" },
            { 1600, "010011010" }, { 1664, "011000" }, { 1728, "010011011" }};

        private static readonly Dictionary<int, string> BlackTerminatingCodes = new Dictionary<int, string>
        {
            { 0, "0000110111" }, { 1, "010" }, { 2, "11" }, { 3, "10" },
            { 4, "011" }, { 5, "0011" }, { 6, "0010" }, { 7, "00011" },
            { 8, "000101" }, { 9, "000100" }, { 10, "0000100" }, { 11, "0000101" },
            { 12, "0000111" }, { 13, "00000100" }, { 14, "00000111" }, { 15, "000011000" },
            { 16, "0000010111" }, { 17, "0000011000" }, { 18, "0000001000" }, { 19, "00001100111" },
            { 20, "00001101000" }, { 21, "00001101100" }, { 22, "00000110111" }, { 23, "00000101000" },
            { 24, "00000010111" }, { 25, "00000011000" }, { 26, "000011001010" }, { 27, "000011001011" },
            { 28, "000011001100" }, { 29, "000011001101" }, { 30, "000001101000" }, { 31, "000001101001" },
            { 32, "000001101010" }, { 33, "000001101011" }, { 34, "000011010010" }, { 35, "000011010011" },
            { 36, "000011010100" }, { 37, "000011010101" }, { 38, "000011010110" }, { 39, "000011010111" },
            { 40, "000001101100" }, { 41, "000001101101" }, { 42, "000011011010" }, { 43, "000011011011" },
            { 44, "000001010100" }, { 45, "000001010101" }, { 46, "000001010110" }, { 47, "000001010111" },
            { 48, "000001100100" }, { 49, "000001100101" }, { 50, "000001010010" }, { 51, "000001010011" },
            { 52, "000000100100" }, { 53, "000000110111" }, { 54, "000000111000" }, { 55, "000000100111" },
            { 56, "000000101000" }, { 57, "000001011000" }, { 58, "000001011001" }, { 59, "000000101011" },
            { 60, "000000101100" }, { 61, "000001011010" }, { 62, "000001100110" }, { 63, "000001100111" }
        };
        private static readonly Dictionary<int, string> BlackMakeupCodes = new Dictionary<int, string>
        {
            { 64, "0000001111" }, { 128, "000011001000" }, { 192, "000011001001" }, { 256, "000001011011" },
            { 320, "000000110011" }, { 384, "000000110100" }, { 448, "000000110101" }, { 512, "0000001101100" },
            { 576, "0000001101101" }, { 640, "0000001001010" }, { 704, "0000001001011" }, { 768, "0000001001100" },
            { 832, "0000001001101" }, { 896, "0000001110010" }, { 960, "0000001110011" }, { 1024, "0000001110100" },
            { 1088, "0000001110101" }, { 1152, "0000001110110" }, { 1216, "0000001110111" }, { 1280, "0000001010010" },
            { 1344, "0000001010011" }, { 1408, "0000001010100" }, { 1472, "0000001010101" }, { 1536, "0000001011010" },
            { 1600, "0000001011011" }, { 1664, "0000001100100" }, { 1728, "0000001100101" }};
        private static readonly Dictionary<int, string> BigCodes = new Dictionary<int, string>
              {
            { 1792, "00000001000" }, { 1856, "00000001100" }, { 1920, "00000001101" }, { 1984, "000000010010" },
            { 2048, "000000010011" }, { 2112, "000000010100" }, { 2176, "000000010101" }, { 2240, "000000010110" },
            { 2304, "000000010111" }, { 2368, "000000011100" }, { 2432, "000000011101" }, { 2496, "000000011110" },
            { 2560, "000000011111" }};
        private const string EOL = "000000000001";
        //fax radi sa crnim i belim pikselima iskljucivo, ppa se slika pojadnostavljenja radi konvertuje u black and white
        public static Bitmap toBW(Bitmap source)
        {
            Bitmap BWbitmap = new Bitmap(source.Width, source.Height);
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    Color originalColor = source.GetPixel(x, y);

                    double r = originalColor.R * 0.3;
                    double g = originalColor.G * 0.59;
                    double b = originalColor.B * 0.11;
                    double gray = r + g + b;
                    if (gray < 127)
                        BWbitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    else BWbitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                }
            }
            return BWbitmap;
        }
       

        public string scanDocument(int[,] mat)
        {
            string code = "";
            //bool first = true;
            for(int i=0; i<mat.GetLength(0); i++) //za redove
            {
                int br = 0;
                int j = 0;
                while(j <= mat.GetLength(1)-2)
                {

                    if (mat[i, j] == mat[i, j + 1]) br++;
                    else
                    {
                        //naisli smo na promenu
                        br++;
                        if (mat[i, j] == 0) code += findCode(br, WhiteTerminatingCodes, WhiteMakeupCodes);
                        else code += findCode(br, BlackTerminatingCodes, BlackMakeupCodes);
                        br = 0;
                       
                    }
                    j++;
                }
                if(j== mat.GetLength(1) - 1)
                {
                    //poslednji element
                    if (mat[i, j - 1] == mat[i, j])
                    {
                        br++;
                        if (mat[i, j] == 0) code += findCode(br, WhiteTerminatingCodes, WhiteMakeupCodes);
                        else code += findCode(br, BlackTerminatingCodes, BlackMakeupCodes);
                    }
                    else
                    {
                        if (mat[i, j] == 0) code += findCode(1, WhiteTerminatingCodes, WhiteMakeupCodes);
                        else code += findCode(1, BlackTerminatingCodes, BlackMakeupCodes);
                    }
                }
                
                code += EOL;
                
            }
            return code;
        }
        public string findCode(int number, Dictionary<int, string> terminating, Dictionary<int, string> makeup)
        {
            if(number<64)
            {
                return terminating[number];
            }
            else
            {
                int makeupPart = number / 64 *64;
                int terminatingPart = number % 64;
                return makeup[makeupPart] + terminating[terminatingPart];
            }
        }

        public int[,] readFax(string code, int rows, int cols)
    {

            //lukin

            int[,] mat = new int[rows, cols];
            int row = 0, col = 0;
            int index = 0;
            int r = 0;

            Dictionary<string, int> whiteTerminatingCodes = WhiteTerminatingCodes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            Dictionary<string, int> blackTerminatingCodes = BlackTerminatingCodes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            Dictionary<string, int> whiteMakeupCodes = WhiteMakeupCodes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            Dictionary<string, int> blackMakeupCodes = BlackMakeupCodes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            Dictionary<string, int> bigCodes = BigCodes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            while (r < rows && index < code.Length)
            {
                int lineEndIndex = code.IndexOf(EOL, index);
                if (lineEndIndex == -1) break;

                ReadOnlySpan<char> line = code.AsSpan(index, lineEndIndex - index);
                index = lineEndIndex + 12;

                int local = 0;
                bool isWhite = true;
                bool wasMakeup = false;
                bool wasBig = false;

                while (local < line.Length)
                {
                    var terminatingCodes = isWhite ? whiteTerminatingCodes : blackTerminatingCodes;
                    var makeupCodes = isWhite ? whiteMakeupCodes : blackMakeupCodes;
                    bool sequenceFound = false;
                    int sequenceLength = 2;

                    while (!sequenceFound && local + sequenceLength <= line.Length)
                    {
                        var currentSequence = line.Slice(local, sequenceLength).ToString();
                        wasMakeup = false;
                        wasBig = false;
                        //if(bigCodes.TryGetValue(currentSequence, out int runLen))
                        //{
                        //    if (runLen <= cols)
                        //    {

                        //    }
                        //}
                        if (makeupCodes.TryGetValue(currentSequence, out int rle))
                        { 
                            if (rle <= cols)
                            {
                                //nasao makeup umnozak od 64
                                for (int i = 0; i < rle; i++)
                                {
                                    if (row >= rows) break;
                                    mat[row, col] = isWhite ? 0 : 1;
                                    col++;
                                    if (col >= cols)
                                    {
                                        col = 0;
                                        row++;
                                        if (row >= rows) break;
                                    }
                                }
                                int termlenght = 2;
                                local += sequenceLength;
                                bool termFound=false;
                                while (!termFound && local + termlenght <= line.Length)
                                {
                                    var termSequence = line.Slice(local, termlenght).ToString();
                                    if (terminatingCodes.TryGetValue(termSequence, out int rlen))
                                    {
                                        for (int i = 0; i < rlen; i++)
                                        {
                                            if (row >= rows) break;
                                            mat[row, col] = isWhite ? 0 : 1;
                                            col++;
                                            if (col >= cols)
                                            {
                                                col = 0;
                                                row++;
                                                if (row >= rows) break;
                                            }
                                        }
                                        local += termlenght;
                                        isWhite = !isWhite;
                                        termFound = true;
                                        sequenceFound = true;
                                    }
                                    else termlenght++;
                                    
                                }
                                wasMakeup = true;
                            }
                           
                            
                        }
                        if (terminatingCodes.TryGetValue(currentSequence, out int runLength) &&!wasMakeup)
                        {
                            
                            //if (wasMakeup) isWhite = !isWhite;
                            for (int i = 0; i < runLength; i++)
                            {
                                if (row >= rows) break;
                                mat[row, col] = isWhite ? 0 : 1;
                                col++;
                                if (col >= cols)
                                {
                                    col = 0;
                                    row++;
                                    if (row >= rows) break;
                                }
                            }
                            local += sequenceLength;
                            isWhite = !isWhite;
                            sequenceFound = true;
                            //wasMakeup = false;
                        }
                        else
                        {
                            sequenceLength++;
                        }
                    }
                    if (!sequenceFound) break;
                }
                r++;
            }

            return mat;

        }



    }
}
