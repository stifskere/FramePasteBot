using Discord.Interactions;

namespace FPB.Commands;

[Group("hardware", "Hardware group")]
public class Hardware : InteractionModuleBase<SocketInteractionContext>
{
    [Group("cpu", "Hardware cpu group")]
    public class cpus : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("list", "Displays a list from a keyword")]
        public async Task ListAsync()
        {
            //doing hardware stuff
        }

        [SlashCommand("find", "Displays the exact hardware part if found")]
        public async Task FindAsync()
        {
        
        }
    }

    [Group("gpu", "Hardware gpu group")]
    public class gpus : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("list", "Displays a list from a keyword")]
        public async Task ListAsync()
        {
        
        }

        [SlashCommand("find", "Displays the exact hardware part if found")]
        public async Task FindAsync()
        {
        
        }
    }
}