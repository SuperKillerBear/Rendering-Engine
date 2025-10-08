using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Components
{
    public class BoxCollider : Collider
    {
        public Vector3D<float> size = new Vector3D<float>(1,1,1);
        public Vector3D<float> center = new Vector3D<float>(0,0,0);
    }
}
