namespace ConsoleApp
{
    //A state object from which the use case objects are derived. Override update to create each use case
    abstract class State
    {
        public abstract Type Update();
    }
}
