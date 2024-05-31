using System;
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
            { 320, "0011011" }, { 384, "0100111" }, { 448, "0110010" }, { 512, "0110110" },
            { 576, "010000" }, { 640, "0000110" }, { 704, "0001100" }, { 768, "011000" },
            { 832, "0000100" }, { 896, "0000111" }, { 960, "0001000" }, { 1024, "0001010" },
            { 1088, "0001011" }, { 1152, "0001101" }, { 1216, "0001110" }, { 1280, "0010000" },
            { 1344, "0010001" }, { 1408, "0010010" }, { 1472, "0010011" }, { 1536, "0010100" },
            { 1600, "0010101" }, { 1664, "0010110" }, { 1728, "0010111" }, 
            // Additional codes for higher multiples (not strictly part of standard but for completeness)
            { 1792, "00000001000" }, { 1856, "00000001100" }, { 1920, "00000001101" }
        };

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
            { 320, "00000011001" }, { 384, "00000011010" }, { 448, "00000011011" }, { 512, "00000011100" },
            { 576, "00000011101" }, { 640, "00000011110" }, { 704, "00000011111" }, { 768, "00001100000" },
            { 832, "00001100001" }, { 896, "00001100010" }, { 960, "00001100011" }, { 1024, "00000101100" },
            { 1088, "000000100101" }, { 1152, "000000100110" }, { 1216, "000000100111" }, { 1280, "000000101000" },
            { 1344, "000000101001" }, { 1408, "000000101010" }, { 1472, "000000101011" }, { 1536, "000000101100" },
            { 1600, "000000101101" }, { 1664, "000000101110" }, { 1728, "000000101111" }, { 1792, "000000110000" },
            { 1856, "000000110001" }, { 1920, "000000110010" }
        };

        private const string EOL = "000000000001";
        //fax radi sa crnim i belim pikselima iskljucivo, ppa se slika pojadnostavljenja radi konvertuje u black and white
        private Bitmap toBW(Bitmap source)
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
            for(int i=0; i<mat.GetLength(0); i++) //za redove
            {
                int br = 1;
                int j = 0;
                while(j < mat.GetLength(1)-1)
                {

                    if (mat[i, j] == mat[i, j + 1]) br++;
                    else
                    {
                        //naisli smo na promenu
                        if (mat[i, j] == 0) code += findCode(br, WhiteTerminatingCodes, WhiteMakeupCodes);
                        else code += findCode(br, BlackTerminatingCodes, BlackMakeupCodes);
                        br = 1;
                    }
                    j++;
                }
                if(j== mat.GetLength(1) - 1)
                {
                    //poslednji element
                    if (mat[i, j-1] == mat[i, j])
                    {
                        br++;
                        if (mat[i, j] == 0) code += findCode(br, WhiteTerminatingCodes, WhiteMakeupCodes);
                        else code += findCode(br, BlackTerminatingCodes, BlackMakeupCodes);
                    }
                    if (mat[i, j] == 0) code+= findCode(1, WhiteTerminatingCodes, WhiteMakeupCodes);
                    else code += findCode(1, BlackTerminatingCodes, BlackMakeupCodes);
                }
                br = 1;
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
                int makeupPart = number / 64;
                int terminatingPart = number % 64;
                return makeup[makeupPart] + terminating[terminatingPart];
            }
        }

        public int[,] readFax(string code, int rows, int cols)
    {
        int[,] mat = new int[rows, cols];
        int row = 0, col = 0;
        int index = 0;
        string line = "";
        int r = 0;
            while (r < rows)
            {
                line = "";
                while (code.Substring(index, 12) != EOL)
                {
                    line = line + code[index];
                    index++;
                }
                index += 12;
                
                // Variables to keep track of decoding progress
                bool isWhite = true;
                int local = 0;
                string[] tmp = new string[50];
                int s = 0;

                while (local < line.Length)
                {
                    // Choose the appropriate dictionary based on color
                    Dictionary<int, string> terminatingCodes = isWhite ? WhiteTerminatingCodes : BlackTerminatingCodes;

                    // Try to find the current sequence in the terminating codes
                    int sequenceLength = 2;
                    bool sequenceFound = false;
                    string currentSequence = "";
                    while (!sequenceFound && local + sequenceLength <= line.Length)
                    {
                        currentSequence = line.Substring(local, sequenceLength);
                        if (terminatingCodes.ContainsValue(currentSequence))
                        {
                            sequenceFound = true;
                            tmp[s] = currentSequence;
                            s++;
                        }
                        else
                        {
                            sequenceLength++;
                            if (local + sequenceLength > line.Length) // Check if sequence is longer than column count
                            {
                                sequenceFound = true; // Treat this as invalid and move to the next position
                            }
                        }
                    }

                    if (sequenceFound)
                    {
                        // Decode the sequence and update matrix
                        foreach (var entry in terminatingCodes)
                        {
                            if (entry.Value == currentSequence)
                            {
                                // Check if the key + current column exceeds cols
                                if (col + entry.Key > cols)
                                {
                                    // Switch to black and backtrack
                                    isWhite = false;
                                    //col = previousCol;
                                    //row = previousRow;
                                }
                                else
                                {
                                    // Update matrix based on whether it's white or black
                                    if (isWhite)
                                    {
                                        // White sequence, add zeroes
                                        for (int i = 0; i < entry.Key; i++)
                                        {
                                            mat[row, col] = 0;
                                            col++;
                                            if (col >= cols)
                                            {
                                                col = 0;
                                                row++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Black sequence, add ones
                                        for (int i = 0; i < entry.Key; i++)
                                        {
                                            mat[row, col] = 1;
                                            col++;
                                            if (col >= cols)
                                            {
                                                col = 0;
                                                row++;
                                            }
                                        }
                                    }

                                    // Move to the next position
                                    local += sequenceLength;
                                    isWhite = !isWhite; // Switch colors
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        // If sequence not found, switch to the other color and continue
                        // isWhite = !isWhite;
                    }
                }
                r++;
            }

        return mat;
    }
        
        public string sendFax(Bitmap source)
        {
            Bitmap bitmap = toBW(source);
            var encoded = new StringBuilder();
            int height = bitmap.Height;
            int width = bitmap.Width;

            for (int i = 0; i < height; i++)
            {
                int runLength = 0;
                int currentColor = 0; // False for white, True for black

                for (int j = 0; j < width; j++)
                {
                    Color c= bitmap.GetPixel(j,i);
                    int b = c.R;
                    if (b == currentColor)
                    {
                        runLength++;
                    }
                    else
                    {
                        // Encode the run
                        if (currentColor==1)
                        {
                            // Encode black run
                            EncodeRun(encoded, runLength, BlackTerminatingCodes, BlackMakeupCodes);
                        }
                        else
                        {
                            // Encode white run
                            EncodeRun(encoded, runLength, WhiteTerminatingCodes, WhiteMakeupCodes);
                        }

                        // Switch color and reset run length
                        currentColor = (currentColor+1)%2;
                        runLength = 1;
                    }
                }

                // Encode the last run in the row
                if (currentColor == 1)
                {
                    // Encode black run
                    EncodeRun(encoded, runLength, BlackTerminatingCodes, BlackMakeupCodes);
                }
                else
                {
                    // Encode white run
                    EncodeRun(encoded, runLength, WhiteTerminatingCodes, WhiteMakeupCodes);
                }

                // Append End of Line (EOL)
                encoded.Append("000000000001");
            }

            return encoded.ToString();
        }

        private void EncodeRun(StringBuilder encoded, int runLength, Dictionary<int, string> terminatingCodes, Dictionary<int, string> makeupCodes)
        {
            while (runLength >= 64)
            {
                int makeupLength = (runLength / 64) * 64;
                encoded.Append(makeupCodes[makeupLength]);
                runLength -= makeupLength;
            }
            if (runLength > 0)
            {
                encoded.Append(terminatingCodes[runLength]);
            }
        }
        public bool[,] DecodeBitmap(string encodedBitmap, int width, int height)
        {
            bool[,] bitmap = new bool[height, width];
            int currentRow = 0;
            int currentColumn = 0;

            // Iterate through the encoded bitmap
            for (int i = 0; i < encodedBitmap.Length;)
            {
                // Search for End of Line (EOL) code
                if (encodedBitmap.Substring(i, 12) == "000000000001")
                {
                    // Move to the next row
                    currentRow++;
                    currentColumn = 0;
                    i += 12; // Skip EOL code
                    continue;
                }

                // Check for black run
                if (encodedBitmap[i] == '1')
                {
                    int runLength = DecodeRunLength(encodedBitmap.Substring(i), BlackTerminatingCodes, BlackMakeupCodes);
                    for (int j = 0; j < runLength; j++)
                    {
                        bitmap[currentRow, currentColumn++] = true; // Set black pixel
                        if (currentColumn >= width)
                        {
                            currentColumn = 0; // Move to the next row if at the end of a row
                            currentRow++;
                        }
                    }
                }
                else // White run
                {
                    int runLength = DecodeRunLength(encodedBitmap.Substring(i), WhiteTerminatingCodes, WhiteMakeupCodes);
                    for (int j = 0; j < runLength; j++)
                    {
                        bitmap[currentRow, currentColumn++] = false; // Set white pixel
                        if (currentColumn >= width)
                        {
                            currentColumn = 0; // Move to the next row if at the end of a row
                            currentRow++;
                        }
                    }
                }

                // Move to the next run in the encoded bitmap
                i += FindNextRunLength(encodedBitmap.Substring(i));
            }

            return bitmap;
        }

        private int FindNextRunLength(string encodedSubstring)
        {
            // Iterate through the encoded string to find the next run length
            int i = 0;
            while (i < encodedSubstring.Length && encodedSubstring[i] == '0')
            {
                i++;
            }
            while (i < encodedSubstring.Length && encodedSubstring[i] == '1')
            {
                i++;
            }
            return i;
        }

        private int DecodeRunLength(string encodedSubstring, Dictionary<int, string> terminatingCodes, Dictionary<int, string> makeupCodes)
        {
            // Try to match the encoded substring with makeup codes first
            foreach (var makeupCode in makeupCodes)
            {
                if (encodedSubstring.StartsWith(makeupCode.Value))
                {
                    return makeupCode.Key + DecodeRunLength(encodedSubstring.Substring(makeupCode.Value.Length), terminatingCodes, makeupCodes);
                }
            }

            // If no makeup code matches, try to match with terminating codes
            foreach (var terminatingCode in terminatingCodes)
            {
                if (encodedSubstring.StartsWith(terminatingCode.Value))
                {
                    return terminatingCode.Key;
                }
            }

            // If no match found, return 0
            return 0;
        }

    }
}
