using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fax
{
    internal class Huffman
    {
        public class Node : IComparable<Node>
        {
            public string Symbol { get; set; }
            public int Frequency { get; set; }
            public Node? Left { get; set; }
            public Node? Right { get; set; }
            public Node? Parent { get; set; }
            public bool isLeaf { get; set; }
            public string? Code { get; set; }

            public Node(string val, int frequency)
            {
                Symbol = val;
                Frequency = frequency;
            }
            public Node(string val)
            {
                Symbol = val;
                Frequency = 1;
                Right = Left = Parent = null;
                isLeaf = true;
                Code = "";
            }

            public Node(Node node1, Node node2)
            {
                Code = "";
                isLeaf = false;
                Parent = null;
                if (node1.Frequency >= node2.Frequency)
                {
                    Right = node1;
                    Left = node2;
                    Right.Parent = Left.Parent = this;
                    Symbol = node1.Symbol + node2.Symbol;
                    Frequency = node1.Frequency + node2.Frequency;
                }
                else
                {
                    Right = node2;
                    Left = node1;
                    Right.Parent = Left.Parent = this;
                    Symbol = node2.Symbol + node1.Symbol;
                    Frequency = node2.Frequency + node1.Frequency;
                }
            }

            public int CompareTo(Node other)
            {
                return Frequency.CompareTo(other.Frequency);
            }

            public void increaseFrequency()
            {
                Frequency++;
            }
        }

        private Dictionary<string, string> encodingTable;
        private Node root;


        //kreira gde su str upareni sa br pojavljanja
        public Dictionary<string, Node> CreateFrequencyTable(string input)
        {
            var frequencyTable = new Dictionary<string, Node>();
            foreach (var ch in input)
            {
                var symbol = ch.ToString();
                if (frequencyTable.ContainsKey(symbol))
                {
                    frequencyTable[symbol].increaseFrequency();
                }
                else
                {
                    Node tmp = new Node(symbol);
                    frequencyTable[symbol] = tmp;
                }
            }
            return frequencyTable.OrderBy(x => x.Value.Frequency).ToDictionary(x => x.Key, x => x.Value);
        }
        private void AssignCodes(Node node, string code)
        {
            if (node != null)
            {
                if (node.isLeaf)
                {
                    node.Code = code;
                    encodingTable[node.Symbol] = code;
                }
                else
                {
                    AssignCodes(node.Left, code + "0");
                    AssignCodes(node.Right, code + "1");
                }
            }
        }

        public void build(string input)
        {

            var frequencyTable = CreateFrequencyTable(input);
            var nodes = new List<Node>(frequencyTable.Values);

            while (nodes.Count > 1)
            {
                nodes = nodes.OrderBy(n => n.Frequency).ToList();

                var left = nodes[0];
                var right = nodes[1];

                var newNode = new Node(left, right);
                nodes.Remove(left);
                nodes.Remove(right);
                nodes.Add(newNode);
            }

            root = nodes[0];
            AssignCodes(root, "");

        }


        public Huffman()
        {
            encodingTable = new Dictionary<string, string>();
        }



        public string Encode(string input)
        {
            build(input);

            var encoded = new System.Text.StringBuilder();
            foreach (var ch in input)
            {
                encoded.Append(encodingTable[ch.ToString()]);
            }

            return encoded.ToString();
        }

        public string Decode(string encodedInput)
        {
            var decoded = new System.Text.StringBuilder();
            var currentNode = root;

            foreach (var bit in encodedInput)
            {
                if (bit == '0')
                {
                    currentNode = currentNode.Left;
                }
                else
                {
                    currentNode = currentNode.Right;
                }

                if (currentNode.isLeaf)
                {
                    decoded.Append(currentNode.Symbol);
                    currentNode = root;
                }
            }

            return decoded.ToString();
        }


    }

}
