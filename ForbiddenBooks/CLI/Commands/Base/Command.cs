namespace ForbiddenBooks.CLI.Commands.Base
{
    public abstract class Command
    { 
        /// <summary>
        /// Invokes the command with the specifed flags.
        /// </summary>
        /// <param name="flags"></param>
        public virtual void Invoke(string[] flags)
        {
            if (flags[0] == "help")
            {
                Help();
                return;
            }
        }

        protected abstract void Help();
    }
}
