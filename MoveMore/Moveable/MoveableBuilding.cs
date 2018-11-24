using Harmony;
using MoveIt;
using UnityEngine;

namespace MoveMore.Moveable
{
    [HarmonyPatch(typeof(MoveableBuilding))]
    [HarmonyPatch("Transform")]
    class MB_Transform
    {
        public static void Postfix(InstanceState instanceState, ref Matrix4x4 matrix4x, float deltaAngle, Vector3 center, bool followTerrain)
        {
            BuildingState state = instanceState as BuildingState;
            Vector3 newPosition = matrix4x.MultiplyPoint(state.position - center);

            if (state.subStates != null)
            {
                foreach (InstanceState subState in state.subStates)
                {
                    if (subState is BuildingState bs)
                    {
                        if (bs.subStates != null)
                        {
                            foreach (InstanceState subSubState in bs.subStates)
                            {
                                Vector3 subPosition = subSubState.position - center;
                                subPosition = matrix4x.MultiplyPoint(subPosition);
                                subPosition.y = subSubState.position.y - state.position.y + newPosition.y;

                                subSubState.instance.Move(subPosition, subSubState.angle + deltaAngle);
                            }
                        }
                    }
                }
            }
        }
    }
}
