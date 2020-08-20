using System;
using System.IO;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Yaml
{
    /// <summary>
    /// Provides methods to create a new configuration file, mainly used by the datafix classes to add new config options
    /// </summary>
    public partial class ConfigCreator : IDisposable
    {
        private Boolean disposedValue;

        /// <summary>
        /// Main writing instance of the class
        /// </summary>
        public StreamWriter Writer { get; private set; }

        /// <summary>
        /// Creates a new instance of the ConfigCreator class
        /// </summary>
        /// <param name="Path">Config file path</param>
        public ConfigCreator(String Path)
        {
            Writer = new StreamWriter(Path);
        }

        /// <summary>
        /// Adds new config option without comment
        /// </summary>
        /// <param name="Option">Config option ID, in the format "namespace.subnamespace.identifier", as in "insanitybot.channels.modlog-channel"</param>
        /// <param name="DefaultValue">The value this option defaults to</param>
        /// <returns></returns>
        public async Task<ConfigCreator> CreateNewConfigOptionAsync(String Option, Object DefaultValue)
        {
            await Writer.WriteLineAsync($"{Option}: {DefaultValue}");
            return this;
        }

        public async Task<ConfigCreator> CreateNewConfigOptionAsync(String Comment, String Option, Object DefaultValue)
        {
            await Writer.WriteLineAsync($"# {Comment}\n{Option}: {DefaultValue}");
            return this;
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                Writer.Close();
                disposedValue = true;
            }
        }

        ~ConfigCreator()
            => Dispose(false);
        

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
