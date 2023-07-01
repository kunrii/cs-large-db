namespace ConsoleApp
{
    internal class Exit : State
    {
        public override Type Update()
        {
            throw new Exception("Do not execute a call to update on Exit");
        }
    }
}
