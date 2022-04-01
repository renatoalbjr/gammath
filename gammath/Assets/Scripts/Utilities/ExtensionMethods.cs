namespace ExtensionMethods
{
    public static class Extensions
    {
        public static string Description(this Rank rank)
        {
            switch (rank)
            {
                case Rank.Pre:
                    return "Pré";

                case Rank.Fundamental:
                    return "Fundamental";
    
                case Rank.Medio:
                    return "Ensino Médio";
    
                case Rank.Graduado:
                    return "Graduado";
    
                case Rank.Mestre:
                    return "Mestre";
    
                case Rank.Doutor:
                    return "Doutor";
    
                case Rank.PosDoutor:
                    return "Pós-Doutor";

                default:
                    return "Unknown";
            }
        }
    }
}