# EaseRoutines-Unity

A simple package that wraps the ease functions from https://easings.net/ into coroutines. 

## Example

```c#
using System.Collections;
using EnvDev;
using UnityEngine;

public class EaseRoutinesExample : MonoBehaviour
{         
    [SerializeField] float m_MoveRightDuration = 1.5f;
    [SerializeField] float m_MoveUpDuration = 1.5f;
    [SerializeField] float m_RightDelta = 5f;
    [SerializeField] float m_UpDelta = 5f;
    
    // This coroutine demonstrates how to use the EaseRoutines class
    IEnumerator MoveRightRoutine()
    {
        var duration = m_MoveRightDuration;
        var startPosition = m_Transform.position;
        var targetPosition = startPosition + Vector3.right * m_RightDelta;

        // An Ease Routine takes a duration parameter and a Lerp Function
        yield return EaseRoutines.CubicOut(duration, t => {
            m_Transform.position = Vector3.Lerp(startPosition, targetPosition, t);
        });

        // Do anything you like once the tween is over
    }

    IEnumerator MoveUpRoutine()
    {
        var duration = m_MoveUpDuration;
        var startPosition = m_Transform.position;
        var targetPosition = startPosition + Vector3.up * m_UpDelta;

        // Here I am using the BounceOut easing function
        yield return EaseRoutines.BounceOut(duration, t => {
            m_Transform.position = Vector3.Lerp(startPosition, targetPosition, t);
        });
    }

    Transform m_Transform;
    // This is not necessary but can be useful if you plan on running multiple coroutines
    CoroutineRunner m_CoroutineRunner;

    void Awake()
    {
        m_Transform = transform;
        m_CoroutineRunner = new CoroutineRunner(this);
    }

    void OnDisable()
    {
        // We want to interrupt anything that might be running when we are disabled to avoid getting errors
        if (m_CoroutineRunner.IsRunning)
            m_CoroutineRunner.Interrupt();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // If the coroutine runner is running we want to interrupt it to avoid stacking the coroutines
            if (m_CoroutineRunner.IsRunning)
            {
                m_CoroutineRunner.Interrupt();
            }
            else
            {
                // You can use the Then() method to chain multiple routines
                m_CoroutineRunner.Run(MoveRightRoutine()).Then(() =>
                {
                    m_CoroutineRunner.Run(MoveUpRoutine());
                });
            }
        }
    }
}
```
