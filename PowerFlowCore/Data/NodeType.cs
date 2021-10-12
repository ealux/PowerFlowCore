namespace PowerFlowCore.Data
{
    /// <summary>
    /// Type of node
    /// PQ -> Load node
    /// PV -> Generation node
    /// Slack -> Slack/Reference node
    /// </summary>
    public enum NodeType
    {
        PQ = 1,     //U - var;    P - const;  Q - const
        PV = 2,     //U - const;  P - const;  Q - var (constrained)
        Slack = 3   //U - const;  P - var;    Q - var
    }
}
