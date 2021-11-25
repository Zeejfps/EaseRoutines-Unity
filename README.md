# EaseRoutines-Unity

A simple package that wraps the ease functions from https://easings.net/ into coroutines. 

## Example

```c#
using System.Collections;
using EnvDev;
using UnityEngine;

public class EaseRoutinesExample : MonoBehaviour
{         
    // This coroutine demonstrates how to use the EaseRoutines package
    IEnumerator MoveTween()
    {
        var duration = 1.5f;
        var myTransform = transform;
        var startPosition = myTransform.position;
        var targetPosition = startPosition + new Vector3(10f, 0f, 0f);

        // An Ease Routine takes a duration parameter and a Lerp Function
        yield return EaseRoutines.CubicOut(duration, t => {
            myTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
        });

        // Do anything you like once the tween is over
    }
    
    // This is not necessary but can be usefull if you plan on running multiple coroutines
    CoroutineRunner m_CoroutineRunner;

    void Awake()
    {
        m_CoroutineRunner = new CoroutineRunner(this);
    }

    void OnDisable()
    {
        // We want to interrupt anything that might be running when we are disabled to avoid getting errors
        m_CoroutineRunner.Interrupt();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_CoroutineRunner.Run(MoveTween());
        }
    }
}
```
