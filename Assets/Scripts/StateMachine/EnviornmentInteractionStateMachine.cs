using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

public class EnviornmentInteractionStateMachine : StateManager<EnviornmentInteractionStateMachine.EEnviornmentInteractionState>
{
    public enum EEnviornmentInteractionState
    {
        Search,
        Approach,
        Rise,
        Touch,
        Reset,

    }

    [SerializeField] private TwoBoneIKConstraint _leftIKConstraint;
    [SerializeField] private TwoBoneIKConstraint _rightIKConstraint;
    [SerializeField] private MultiRotationConstraint _leftRotationConstraint;
    [SerializeField] private MultiRotationConstraint _rightRotationConstraint;

    [SerializeField] private Rigidbody _rigidbody; //Speed
    [SerializeField] private CapsuleCollider _rootcollider; //Dimensions


    //if using char controller grab equivalent data as above (Uncomment if using character controller)
    //[SerializeField] private CharacterController _characterController;

    void Awake()
    {
        ValidateConstraints();
    }

    private void ValidateConstraints()
    {
        Assert.IsNotNull(_leftIKConstraint, "left IK constraint not assigned");
        Assert.IsNotNull(_rightIKConstraint, "right IK constraint not assigned");
        Assert.IsNotNull(_leftRotationConstraint, "left rotation constraint not assigned");
        Assert.IsNotNull(_rightRotationConstraint, "right rotation constraint not assigned");
        Assert.IsNotNull(_rigidbody, "rigidBody not assigned");
        Assert.IsNotNull(_rootcollider, "root collider not assigned");


        //Assert.IsNotNull(_characterController, "character Controller not assigned");
    }



}
