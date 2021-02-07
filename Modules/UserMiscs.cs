using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Android.Modules
{
    public class UserMiscs: ModuleBase<SocketCommandContext>
    {
        [Command("avatar"), Alias("avt")]
        [Summary("Get user avatar")]
        public async Task AvatarAsync([Remainder] SocketGuildUser user = null)
        {
            if (user == null)
            {
                var url = Context.User.GetAvatarUrl(size: 512);
                var embed = new EmbedBuilder()
                    .WithColor(0, 254, 211)
                    .WithAuthor(Context.User.Username + "#" + Context.User.Discriminator)
                    .WithDescription($"[Avatar URL Link]({url})")
                    .WithImageUrl(Context.User.GetAvatarUrl(size: 512))
                    .Build();
                await ReplyAsync(embed: embed);
            }
            else
            {
                var url = user.GetAvatarUrl(size: 512);
                var embed = new EmbedBuilder()
                    .WithColor(0, 254, 211)
                    .WithAuthor(user)
                    .WithDescription($"[Avatar URL Link]({url})")
                    .WithImageUrl(user.GetAvatarUrl(size: 512))
                    .Build();
                await ReplyAsync(embed: embed);
            }
        }
    }
}
