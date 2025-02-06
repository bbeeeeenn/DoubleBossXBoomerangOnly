using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace TShockPlugin.Events;

public class OnGetData : Models.Event
{
    static readonly short[] MinionProjectiles =
    {
        ProjectileID.AbigailCounter,
        ProjectileID.AbigailMinion,
        ProjectileID.BabyBird,
        ProjectileID.FlinxMinion,
        ProjectileID.BabySlime,
        ProjectileID.VampireFrog,
        ProjectileID.Hornet,
        ProjectileID.FlyingImp,
        ProjectileID.VenomSpider,
        ProjectileID.JumperSpider,
        ProjectileID.DangerousSpider,
        ProjectileID.BatOfLight,
        ProjectileID.OneEyedPirate,
        ProjectileID.SoulscourgePirate,
        ProjectileID.PirateCaptain,
        ProjectileID.Smolstar,
        ProjectileID.Retanimini,
        ProjectileID.Spazmamini,
        ProjectileID.Pygmy,
        ProjectileID.Pygmy2,
        ProjectileID.Pygmy3,
        ProjectileID.Pygmy4,
        ProjectileID.StormTigerGem,
        ProjectileID.StormTigerTier1,
        ProjectileID.StormTigerTier2,
        ProjectileID.StormTigerTier3,
        ProjectileID.DeadlySphere,
        ProjectileID.Raven,
        ProjectileID.UFOMinion,
        ProjectileID.Tempest,
        ProjectileID.StardustDragon1,
        ProjectileID.StardustDragon2,
        ProjectileID.StardustDragon3,
        ProjectileID.StardustDragon4,
        ProjectileID.StardustCellMinion,
        ProjectileID.EmpressBlade,
    };

    public static bool IsMinion(short id)
    {
        return MinionProjectiles.Contains(id);
    }

    public override void Disable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NetGetData.Deregister(plugin, EventMethod);
    }

    public override void Enable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NetGetData.Register(plugin, EventMethod);
    }

    private void EventMethod(GetDataEventArgs args)
    {
        TSPlayer player = TShock.Players[args.Msg.whoAmI];

        using BinaryReader reader = new(
            new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)
        );
        if (args.MsgID == PacketTypes.ProjectileNew)
        {
            var projectileID = reader.ReadInt16();
            if (!IsMinion(projectileID))
                return;
            _ = reader.ReadSingle();
            _ = reader.ReadSingle();
            _ = reader.ReadSingle();
            _ = reader.ReadSingle();
            var playerID = reader.ReadByte();
            player.SendData(PacketTypes.ProjectileDestroy, "", projectileID, playerID);
            player.SendErrorMessage("You can't summon a minion.");
            args.Handled = true;
        }
        if (args.MsgID == PacketTypes.NpcStrike)
        {
            //
        }
    }
}
