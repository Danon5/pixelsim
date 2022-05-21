using UnityEngine;

namespace VoxelSim.DeveloperTools
{
    public sealed class DevController : MonoBehaviour
    {
        private void Update()
        {
            Vector2 moveAxis = new Vector2(
                Input.GetAxisRaw("Horizontal"), 
                Input.GetAxisRaw("Vertical")).normalized;
            
            transform.Translate(moveAxis * ((Input.GetKey(KeyCode.LeftShift) ? 25f : 10f) * Time.deltaTime));
        }
    }
}