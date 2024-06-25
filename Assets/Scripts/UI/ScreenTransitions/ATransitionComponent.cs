using System;
using UnityEngine;

namespace UI.ScreenTransitions
{
    /// <summary>
    /// Screens use ATransitionComponents to animate their in and out transitions
    /// this can be extended to use lerps/animations
    /// </summary>
    public abstract class ATransitionComponent
    {
        /// <summary>
        /// Animate the specified target transform and execute finishedCallback when the animation is done
        /// </summary>
        /// <param name="target">The target transform</param>
        /// <param name="finishedCallback">The callback to be called when the animation is done</param>
        public abstract void Animate(Transform target, Action finishedCallback);
    }
}