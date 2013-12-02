using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;

public class ShasaAI : MonoBehaviour 
{
    public Queue<WeaveOrder> orders = new Queue<WeaveOrder>();
    public ShasaState shasaState;
    public WeaveOrder curOrder;

    public enum ShasaState
    {
        Weaving = 0,
        FindingResources = 1,
    }

    void Update()
    {
 
    }

    public void giveWeaveOrder(WeaveOrder w)
    {
        orders.Enqueue(w);
    }
}

[System.Serializable]
public class WeaveResources
{
    public int _fiber = 0;
    public int _string = 0;
    
    public WeaveResources( int f, int s )
    {
        _fiber = f;
        _string=s;
    }
}

[System.Serializable]
public class WeaveOrder
{
    public static WeaveOrder STRING {
        get {
            return new WeaveOrder(5, new WeaveResources(5, 0));
        }
    }
    public static WeaveOrder ROPE
    {
        get
        {
            return new WeaveOrder(8, new WeaveResources(0, 3));
        }
    }
    public static WeaveOrder SAILCLOTH
    {
        get
        {
            return new WeaveOrder(30, new WeaveResources(15, 4));
        }
    }

    public float time;
    public WeaveResources resources;

    public WeaveOrder(float t, WeaveResources wR)
    {
        time = t;
        resources = wR;
    }
}

public class DeliverResources : Action 
{
    public DeliverResources()
    {
        actionName = "DeliverResources";
    }

    public override ActionResult Start(Agent agent, float deltaTime)
    {
        return ActionResult.SUCCESS;
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
        return ActionResult.SUCCESS;
    }

    public override ActionResult Stop(Agent agent, float deltaTime)
    {
        return ActionResult.SUCCESS;
    }
}