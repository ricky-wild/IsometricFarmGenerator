using UnityEngine.UI;
using UnityEngine;
using wildlogicgames;

namespace IsometricFarmGenerator
{

    public class TouchController : MonoBehaviour
    {


        private void Awake() => Setup();
        private void Setup()
        {

        }
        private void Update()
        {
            
        }
        private void TouchInputUpdate()
        {
            if (Input.touchCount >= 1)
            {
                Touch firstTouch = Input.GetTouch(0);

                // First touch is being held
                if (firstTouch.phase == TouchPhase.Stationary || firstTouch.phase == TouchPhase.Moved)
                {
                    // Check for a second touch
                    if (Input.touchCount >= 2)
                    {
                        Touch secondTouch = Input.GetTouch(1);

                        if (secondTouch.phase == TouchPhase.Began)
                        {
                            print("\nSecond touch began while first touch is held.");
                            // Trigger your action here
                        }
                    }
                }
            }
        }
    }
}
