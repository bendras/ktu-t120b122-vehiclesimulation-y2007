namespace VehicleSimulation
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (VehicleSimulation game = new VehicleSimulation())
            {
                game.Run();
            }
        }
    }
}