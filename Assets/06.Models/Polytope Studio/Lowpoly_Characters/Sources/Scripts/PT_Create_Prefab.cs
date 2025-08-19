using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Polytope
{
    public class PT_Create_Prefab : MonoBehaviour
    {
        public delegate void PreviewCallback();

        public static class TypeOfMesh
        {
            public static readonly string hair = "hair";
            public static readonly string head = "head";
            public static readonly string beard = "beard";
            public static readonly string helmet = "helmet";
            public static readonly string body = "body";
            public static readonly string boots = "boots";
            public static readonly string cape = "cape";
            public static readonly string gauntlets = "gauntlets";
            public static readonly string legs = "legs";
            public static readonly string upper = "upper";
            public static readonly string lower = "lower";

        }

        [System.Serializable]
        public class ShaderProperties_C
        {
            [ColorUsage(true, true)] [SerializeField]  Color color;
            public Color Color { get { return color; } }
        }

        [System.Serializable]
        public class ShaderProperties_CI
        {
            [ColorUsage(true, true)] [SerializeField]  Color color;
            [SerializeField]  Sprite image;

            public Color Color { get { return color; } }
            public Sprite Image { get { return image; } }
        }

        [System.Serializable]
        public class ShaderProperties_CS
        {
            [ColorUsage(true, true)] [SerializeField]  Color color;
            [Range(0f, 1f)] [SerializeField]  float smoothness;
            public Color Color { get { return color; } }
            public float Smoothness { get { return smoothness; } }
        }

        [System.Serializable]
        public class ShaderProperties_CSM
        {
            [ColorUsage(true, true)] [SerializeField]  Color color;
            [Range(0f, 1f)] [SerializeField]  float smoothness;
            [Range(0f, 1f)] [SerializeField]  float metallic;
            public Color Color { get { return color; } }
            public float Smoothness { get { return smoothness; } }
            public float Metallic { get { return metallic; } }
        }

        [System.Serializable]
        public class ShaderProperties_CSP
        {
            [ColorUsage(true, true)] [SerializeField]  Color color;
            [Range(0f, 1f)] [SerializeField]  float size;
            [Range(0f, 1f)] [SerializeField]  float power;
        }

        [System.Serializable]
        public class MeshType
        {
            public MeshType(string type)
            {
                this.type = type;
            }

            [SerializeField]  string type;
            [SerializeField]  int idx = 0;
            [SerializeField]  List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>();

            public string Type { get { return type; } }
            public int Idx { get { return idx; } set { idx = value; } }
            public List<SkinnedMeshRenderer> List { get { return list; } }
        }

        [System.Serializable]
         class MaterialType
        {
            [SerializeField]  List<Material> assets;
            [SerializeField]  Material instance;
            [SerializeField]  int index;
        }

        [SerializeField]
         List<MeshType> meshes = new List<MeshType>
        {
            new MeshType(TypeOfMesh.hair),
            new MeshType(TypeOfMesh.head),
            new MeshType(TypeOfMesh.beard),
            new MeshType(TypeOfMesh.helmet),
            new MeshType(TypeOfMesh.body),
            new MeshType(TypeOfMesh.boots),
            new MeshType(TypeOfMesh.cape),
            new MeshType(TypeOfMesh.gauntlets),
            new MeshType(TypeOfMesh.legs),
            new MeshType(TypeOfMesh.upper),
            new MeshType(TypeOfMesh.lower)
        };
        [SerializeField]  MaterialType material;
        [SerializeField]  bool loaded;
        [SerializeField]  bool tooglehelmet;

        // Shader

        [SerializeField]  ShaderProperties_CS skin;
        [SerializeField]  ShaderProperties_CS eyes;
        [SerializeField]  ShaderProperties_CS hair;
        [SerializeField]  ShaderProperties_CS sclera;
        [SerializeField]  ShaderProperties_CS lips;
        [SerializeField]  ShaderProperties_CS scars;

        [SerializeField]  ShaderProperties_C feathers1;
        [SerializeField]  ShaderProperties_C feathers2;
        [SerializeField]  ShaderProperties_C feathers3;
        [SerializeField]  ShaderProperties_C cloth1;
        [SerializeField]  ShaderProperties_C cloth2;
        [SerializeField]  ShaderProperties_C cloth3;
        [SerializeField]  ShaderProperties_CS leather1;
        [SerializeField]  ShaderProperties_CS leather2;
        [SerializeField]  ShaderProperties_CS leather3;
        [SerializeField]  ShaderProperties_CSM metal1;
        [SerializeField]  ShaderProperties_CSM metal2;
        [SerializeField]  ShaderProperties_CSM metal3;

        [SerializeField]  ShaderProperties_CS gems1;
        [SerializeField]  ShaderProperties_CS gems2;
        [SerializeField]  ShaderProperties_CS gems3;
        [SerializeField]  ShaderProperties_CI coatofarms;

        [SerializeField]  ShaderProperties_CSP light1;
        [SerializeField]  ShaderProperties_CSP light2;
        [SerializeField]  ShaderProperties_CSP light3;

        [Range(0f, 1f)] [SerializeField]  float occlusion;
        [SerializeField]  bool metallicOn;
        [SerializeField]  bool smoothnessOn;
        [SerializeField]  bool gpuinstancing;
        [SerializeField]  bool doubleSidedGI;
        [Range(-1, 5000)] [SerializeField]  int renderQueue;

        [SerializeField]  bool preview;
        [SerializeField]  float time;
        [SerializeField]  bool duplicateMaterial;
    }
}

