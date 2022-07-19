namespace PowerFlowCore.Data
{
    /// <summary>
    /// Type of node
    /// <list>
    /// <item>PQ -> Load node</item>
    /// <item>PV -> Generation node</item>
    /// <item>Slack -> Slack/Reference node</item>
    /// </list>
    /// </summary>
    public enum NodeType: byte
    {
        PQ = 1,     //U - var;    P - const;  Q - const
        PV = 2,     //U - const;  P - const;  Q - var (constrained)
        Slack = 3   //U - const;  P - var;    Q - var
    }
}
