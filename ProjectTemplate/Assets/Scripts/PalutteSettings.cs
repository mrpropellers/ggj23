using UnityEngine;

namespace GGJ23
{
    [CreateAssetMenu(fileName = "PalutteSettings", menuName = "Palutte", order = 0)]
    public class PalutteSettings : ScriptableObject
    {
        public Material material;

        public Texture LUTTexture;

        public int gridWidth = 16;
        public int gridHeight = 16;

        public int pixelsWidth = 200;
        public int pixelsHeight = 150;

        private int activeWidth = 200;
        private int activeHeight = 200;

        public bool autoSetWidth = false;
        public bool autoSetHeight = false;
        public bool matchCamSize = false;

        [Range(0f, 0.5f)]
        public float ditherAmount = 0.1f;

        [Range(0.5f, 2f)]
        public float pixelStretch = 1f;

        public bool jaggiesAreGood = true;
    }
}
