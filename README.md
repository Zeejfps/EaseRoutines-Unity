# EaseRoutines-Unity

A simple package that wraps the ease functions from https://easings.net/ into coroutines. 

## Example

```c#
using EnvDev;

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

    Coroutine m_Tween;

    void PlayTween(IEnumerator tween) 
    {
        if (m_Tween != null)  
        {
            StopCoroutine(m_Tween);
        }
        m_Tween = StartCoroutine(tween);
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayTween(MoveTween());
        }
    }
}
```
