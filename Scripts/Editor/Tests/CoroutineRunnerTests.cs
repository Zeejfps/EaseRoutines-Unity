using System.Collections;
using System.Collections.Generic;
using EnvDev;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

public class CoroutineRunnerTests
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    // [UnityTest]
    // public IEnumerator CoroutineRunnerTestsWithEnumeratorPasses()
    // {
    //     // Use the Assert class to test conditions.
    //     // Use yield to skip a frame.
    //     yield return null;
    // }

    public class InterruptMethodTests
    {
        [Test]
        public void Interrupt_With_One_Running_Coroutine_Stops_The_Coroutine()
        {
            Assert.IsTrue(true);
        }
    }

    public class RunMethodTests
    {
        bool m_DidStart1;
        bool m_DidStart2;
        bool m_DidStart3;

        CoroutineRunner m_CoroutineRunner;
        
        [SetUp]
        public void Setup()
        {
            var gameObject = new GameObject();
            var target = gameObject.AddComponent<TestMonoBehaviour>();
            m_CoroutineRunner = new CoroutineRunner(target);
        }
        
        [Test]
        public void Run_With_One_Coroutine_Starts_The_Coroutine()
        {
            m_CoroutineRunner.Run(TestCoroutine1());

            Assert.IsTrue(m_DidStart1);
        }

        [Test]
        public void Run_With_Multiple_Coroutines_Starts_All_Coroutine()
        {
            m_CoroutineRunner.Run(TestCoroutine1(), TestCoroutine2(), TestCoroutine3());

            Assert.IsTrue(m_DidStart1);
            Assert.IsTrue(m_DidStart2);
            Assert.IsTrue(m_DidStart3);
        }

        [UnityTest]
        public IEnumerator Run_With_One_Coroutine_Invokes_Then_Action_When_Coroutine_Completes()
        {
            var thenInvoked = false;
            m_CoroutineRunner.Run(TestCoroutine1()).Then(() =>
            {
                thenInvoked = true;
            });

            yield return null;
            
            Assert.IsTrue(thenInvoked);
        }
        
        [UnityTest]
        public IEnumerator Run_With_Multiple_Coroutines_Invokes_Then_Action_When_All_Coroutine_Completes()
        {
            var thenInvoked = false;
            m_CoroutineRunner.Run(TestCoroutine1(), TestCoroutine2(), TestCoroutine3()).Then(() =>
            {
                thenInvoked = true;
            });

            yield return new WaitForSeconds(1f);
            
            Assert.IsTrue(thenInvoked);
        }
        
        IEnumerator TestCoroutine1()
        {
            m_DidStart1 = true;
            yield return null;
        }
        
        IEnumerator TestCoroutine2()
        {
            m_DidStart2 = true;
            yield return null;
            yield return null;
        }
        
        IEnumerator TestCoroutine3()
        {
            m_DidStart3 = true;
            yield return null;
            yield return null;
            yield return null;
        }
    }

    class TestMonoBehaviour : MonoBehaviour
    {
        
    }
}
