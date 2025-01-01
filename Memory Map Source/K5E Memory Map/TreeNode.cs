using K5E_Memory_Map.UIModule;
using System.Collections.Generic;

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace K5E_Memory_Map
{


    public class TreeNode
    {

        string[,] TagData = {
        {"Special", "Yellow", "Special"}, //0

        //IM
        {"Bike", "LightBlue", "Bike"}, //1
        {"Cell", "LightBlue", "Cell"},  //2
        {"Exit", "LightGray", "Leave Cave"}, //3
        {"Cam", "LightGray", "Cam Stabalise"}, //4
        {"Drop", "LightGray", "Drop from IM"}, //5
        {"Gate", "SandyBrown", "Gate Clip"}, //6
        {"S-Drop", "SandyBrown", "Seam Drop"}, //7
        {"S-Land", "SandyBrown", "Seam Land"}, //8
        {"Arrive", "Aquamarine", "Arrive"},//9
        {"Start", "White", "Load Save/End CS"}, //10

        //Void
        
        {"Swim", "Cyan", "Swim Start"}, //11
        {"Turn", "Cyan", "Change Swim"}, //12
        {"Swimming", "Cyan", "Swimming"}, //13
        {"Enter", "Magenta", "Before Enter"},//14
        {"Reload", "Magenta", "Reload Map"},//15
        {"Pass", "LightGreen", "Success"},//16
        {"Fail", "Red", "Fail"},//17
        {"Height", "White", "Change Swim Height"},//18
        {"Deload", "White", "Deload Map"}, //19

        //Krystal
        {"Galleon", "White", ""},//20
        {"Scales", "White", ""},//21
        {"Wall 1", "White", ""},//22
        {"Pickup", "White", ""},//23
        {"Earthwalker", "White", ""},//24
        {"Ladder", "White", ""},//25
        {"Jellyfish", "White", ""},//26
        {"K1", "White", ""},//27
        {"Submit K1", "White", ""},//28
        {"Flight", "White", ""},//29

        {"Reload", "White", "Reload CC"},//30

        //TTH
        {"Staff", "White", "Staff"},//31
        {"SharpClaw", "Orange", "End Sharpclaw"},//32
        {"Fireblast", "Tomato", "Get Fireblast"},//33
        {"Scarabs", "Gold", "Get Scarabs"},//34
        {"Spores", "Violet", "Get Spore"},//35
        {"Queen", "DarkTurquoise", "Talk Queen"},//36
        {"Shop", "MediumSeaGreen", "Enter Shop"},//37
        {"Candy", "Peru", "Rock Candy"},//38
        {"Magic", "Aqua", "Pickup Magic"},//39
        {"Bomb", "Fuchsia", "Explode Bomb"},//40
        {"WS", "Aquamarine", "Talk Warpstone"},//41

        //Mammoth
        {"", "White", ""},//42
        {"", "White", ""},//43
        {"", "White", ""},//44
        {"", "White", ""},//45
        {"", "White", ""},//46
        {"", "White", ""},//47
        {"", "White", ""},//48
        {"", "White", ""},//49
        {"", "White", ""},//50
        {"", "White", ""},//51
        {"", "White", ""},//52
        {"", "White", ""} //53


        };

        public string Mem { get; set; }
        public List<String> ParentIds { get; set; } = new();
        public List<String> ChildIds { get; set; } = new();
        public int? Tag { get; set; } = null;
        public string? Text { get; set; } = null;
        public string? TagText { get; set; } = null;
        public List<float[]> MountCoords { get; set; } = new ();
        public bool Stated = false;
        public bool? PracPath = null;
        public string Colour = "White";


        [JsonIgnore]
        public List<TreeNode> Parents { get; set; }
        [JsonIgnore]
        public List<TreeNode> Children { get; set; }


        public TreeNode(string mem, Dictionary<string, TreeNode> NodeHash, TreeNode parent = null)
        {

            Mem = mem;
            Parents = new List<TreeNode>();
            if (parent != null)
            {
                AddParent(parent);
                parent.AddChild(this);
            }

            Children = new List<TreeNode>();
            NodeHash.Add(Mem, this);

        }

        public void AddChild(TreeNode child)
        {
            if (!Children.Contains(child))
            {
                Children.Add(child);
            }
        }

        public void AddParent(TreeNode parent)
        {
            if (!Parents.Contains(parent))
            {
                Parents.Add(parent);
            }
        }

        public void AddTag(int? tag)
        {
            Tag = tag;
            TagConvert(Tag);
            TagColour(Tag);
        }

        public void TagColour(int? Tag)
        {
            if (Tag == null)
            {
                Colour = "White";
            }
            else
            {
                Colour = TagData[(int)Tag, 1];
            }
        }

        public void TagConvert(int? ID)
        {
            if (ID == null)
            {
                TagText = null;
            }
            else
            {
                TagText = TagData[(int)ID, 2];
            }
        }

        public void RemChild(TreeNode child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
            }
        }

        public void RemParent(TreeNode parent)
        {
            if (Parents.Contains(parent))
            {
                Parents.Remove(parent);
            }
        }

        public void AddMount(float x, float y, float z)
        {
            MountCoords.Add(new float[] {x,y,z} );
        }

        public void DelNode(Dictionary<string, TreeNode> NodeHash)
        {
            foreach (var P in Parents)
            {
                P.RemChild(this);
            }
            foreach (var C in Children)
            {
                C.RemParent(this);
            }
            Parents.Clear();
            Children.Clear();
            NodeHash.Remove(Mem);
        }

        public void DelBelow(Dictionary<string, TreeNode> NodeHash)
        {
            foreach (var P in Parents)
            {
                P.RemChild(this);
            }

            DelBNode(NodeHash);
        }

        public void DelBNode(Dictionary<string, TreeNode> NodeHash)
        {
            foreach (var C in Children)
            {

                C.DelBNode(NodeHash);
            }
            Parents.Clear();
            Children.Clear();
            NodeHash.Remove(Mem);
        }
    }
}