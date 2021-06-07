using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Threading;

// This file contains registration of aspects that are applied to several classes of this project.
[assembly: DeadlockDetectionPolicy]
[assembly:
    Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public,
        AttributeTargetMemberAttributes = MulticastAttributes.Public)]