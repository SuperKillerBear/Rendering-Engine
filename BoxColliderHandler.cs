using System.Drawing;
using RenderingEngine.Components;
using RenderingEngine.GameObjects;
using Silk.NET.Maths;

namespace RenderingEngine 
{
    public static class BoxColliderHandler 
    {
        private static int bufferSize = 150;
        private static BoxColliderComponent?[] ColliderObjBuffer = new BoxColliderComponent[bufferSize];

        private static List<int> unusedIndices = new List<int>();

        private static int pointer = 0;
        public static int AddObj(BoxColliderComponent rb) 
        {
            //Issue, May be repeating objects as if object has both rigidbody and box collider => owner added twice
            //Array.FindIndex(PhysObjBuffer, x => x.Contains("author"));
            //if (obj in PhysObjBuffer) 

            //System to reuse indicies from list
            if (unusedIndices.Count > 0) 
            {
                int reusedIndex = unusedIndices[0];
                unusedIndices.Remove(reusedIndex);

                ColliderObjBuffer[reusedIndex] = rb;
            
                return reusedIndex;
            }

            if (pointer >= bufferSize - 1) 
            { 
                Console.WriteLine("WARN: Collider Obj Buffer Full, Cant add new object!");  
                return -1; 
            }
            
            ColliderObjBuffer[pointer] = rb;

            pointer++;
            return (pointer - 1);
        }

        public static void RemoveObj(BoxColliderComponent obj, int index)
        {
            if (index <= 0) return;
            if (ColliderObjBuffer[index] == obj)
            {
                ColliderObjBuffer[index] = null;
                unusedIndices.Add(index);
            }
        }
        

        
        private static bool Intersects(in Vector3D<float> aMin, in Vector3D<float> aMax, in Vector3D<float> bMin, in Vector3D<float> bMax)
        {
            return
                aMin.X <= bMax.X && aMax.X >= bMin.X &&
                aMin.Y <= bMax.Y && aMax.Y >= bMin.Y &&
                aMin.Z <= bMax.Z && aMax.Z >= bMin.Z;
        }

        public static void CalcColisions()
        {
            // 1) Update AABBs first
            for (int i = 0; i < pointer; i++)
            {
                var box = ColliderObjBuffer[i];
                if (box != null) box.TickCollider();
            }

            List<Contact> contacts = new();

            // 2) Pair checks
            for (int p = 0; p < pointer; p++)
            {
                var a = ColliderObjBuffer[p];

                for (int j = p + 1; j < pointer; j++)
                {
                    var b = ColliderObjBuffer[j];

                    // Skip same owner
                    if (a.Owner == b.Owner)
                        continue;

                    if (!Intersects(a.WorldMin, a.WorldMax, b.WorldMin, b.WorldMax))
                        continue;

                    contacts.Add(MakeAabbContact(a,b));
                }
                
                // Solve (a few iterations helps stacks)
                for (int iter = 0; iter < 6; iter++)
                {
                	for (int i = 0; i < contacts.Count; i++)
                	{
                		var c = contacts[i];
                		ApplyImpulse(ref c);
                		contacts[i] = c;
                	}
                }
                
                // Correct overlap once
                for (int i = 0; i < contacts.Count; i++)
                {
                	var c = contacts[i];
                	PositionalCorrection(ref c);
                	contacts[i] = c;
                }
            }
        }
        
        private static Contact MakeAabbContact(BoxColliderComponent a, BoxColliderComponent b)
        {
        	var aMin = a.WorldMin;
        	var aMax = a.WorldMax;
        	var bMin = b.WorldMin;
        	var bMax = b.WorldMax;
        
        	float ox = MathF.Min(aMax.X, bMax.X) - MathF.Max(aMin.X, bMin.X);
        	float oy = MathF.Min(aMax.Y, bMax.Y) - MathF.Max(aMin.Y, bMin.Y);
        	float oz = MathF.Min(aMax.Z, bMax.Z) - MathF.Max(aMin.Z, bMin.Z);
        
        	// assume you only call this if intersecting
        	var aCenter = (aMin + aMax) * 0.5f;
        	var bCenter = (bMin + bMax) * 0.5f;
        	var d = bCenter - aCenter;
        
        	Vector3D<float> n;
        	float pen;
        
        	if (ox <= oy && ox <= oz)
        	{
        		n = new Vector3D<float>(d.X < 0 ? -1f : 1f, 0f, 0f);
        		pen = ox;
        	}
        	else if (oy <= ox && oy <= oz)
        	{
        		n = new Vector3D<float>(0f, d.Y < 0 ? -1f : 1f, 0f);
        		pen = oy;
        	}
        	else
        	{
        		n = new Vector3D<float>(0f, 0f, d.Z < 0 ? -1f : 1f);
        		pen = oz;
        	}
        
        	return new Contact
        	{
        		ACol = a,
        		BCol = b,
        		ARb = a.Owner.GetComponent<RigidBodyComponent>(), // null => static
        		BRb = b.Owner.GetComponent<RigidBodyComponent>(),
        		Normal = n,
        		Penetration = pen
        	};
        }

        private static void PositionalCorrection(ref Contact c)
        {
        	float invMassA = c.ARb?.massInv ?? 0f;
        	float invMassB = c.BRb?.massInv ?? 0f;
        	float invMassSum = invMassA + invMassB;
        
        	if (invMassSum <= 0f)
        		return;
        
        	const float percent = 0.8f; // 80% of penetration
        	const float slop = 0.01f;	 // small allowance
        
        	float correctionMag = MathF.Max(c.Penetration - slop, 0f) * percent / invMassSum;
        	var correction = c.Normal * correctionMag;
        
        	// Move transforms directly
        	if (invMassA > 0f)
        		c.ACol.Owner.Transform.Translate(-correction * invMassA);
        
        	if (invMassB > 0f)
        		c.BCol.Owner.Transform.Translate(correction * invMassB);
        }

        public static void ApplyImpulse(ref Contact c)
        {
        	float invMassA = c.ARb?.massInv ?? 0f;
        	float invMassB = c.BRb?.massInv ?? 0f;
        	float invMassSum = invMassA + invMassB;
        
        	if (invMassSum <= 0f)
        		return;
        
        	var vA = c.ARb?.Velocity ?? Vector3D<float>.Zero;
        	var vB = c.BRb?.Velocity ?? Vector3D<float>.Zero;
        
        	var rv = vB - vA;
        	float velAlongNormal = rv.X * c.Normal.X + rv.Y * c.Normal.Y + rv.Z * c.Normal.Z;
        
        	// if separating, don’t apply impulse
        	if (velAlongNormal > 0f)
        		return;
        
        	float eA = c.ARb?.restitution ?? 0f;
        	float eB = c.BRb?.restitution ?? 0f;
        	float e = MathF.Min(eA, eB);
        
        	float j = -(1f + e) * velAlongNormal;
        	j /= invMassSum;
        
        	var impulse = c.Normal * j;
        
        	if (c.ARb != null)
        		c.ARb.Velocity = vA - impulse * invMassA;
        
        	if (c.BRb != null)
        		c.BRb.Velocity = vB + impulse * invMassB;
        }

        public static void ClearAll()
        {
            ColliderObjBuffer = new BoxColliderComponent[bufferSize];
            unusedIndices.Clear();
            pointer = 0;
        }

    }
}