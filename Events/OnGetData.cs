using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace TShockPlugin.Events;

public class OnGetData : Models.Event
{
    public static readonly Dictionary<string, DateTime> LastInvalidWeapon = new();
    public static readonly Dictionary<string, DateTime> LastSummonTry = new();
    static readonly int[] MinionProjectiles =
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
    static readonly int[] Boomerangs =
    {
        ItemID.WoodenBoomerang,
        ItemID.FruitcakeChakram,
        ItemID.BloodyMachete,
        ItemID.CombatWrench,
        ItemID.Shroomerang,
        ItemID.EnchantedBoomerang,
        ItemID.Flamarang,
        ItemID.IceBoomerang,
        ItemID.ThornChakram,
        ItemID.Trimarang,
        ItemID.BouncingShield,
        ItemID.LightDisc,
        ItemID.Bananarang,
        ItemID.FlyingKnife,
        ItemID.LightDisc,
        ItemID.PossessedHatchet,
        ItemID.PaladinsHammer,
    };

    public static bool IsBoomerang(int id)
    {
        return Boomerangs.Contains(id);
    }

    public static bool IsMinion(int id)
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
            _ = reader.ReadSingle();
            _ = reader.ReadSingle();
            _ = reader.ReadSingle();
            _ = reader.ReadSingle();
            var ownderID = reader.ReadByte();
            var type = reader.ReadInt16();

            if (!IsMinion(type))
                return;

            player.SendData(PacketTypes.ProjectileDestroy, "", projectileID, ownderID);
            if (
                !LastSummonTry.ContainsKey(player.Name)
                || (DateTime.Now - LastSummonTry[player.Name]).Seconds >= 5
            )
            {
                LastSummonTry[player.Name] = DateTime.Now;
                player.SendErrorMessage("You can't summon a minion.");
            }
            args.Handled = true;
        }

        if (args.MsgID == PacketTypes.NpcStrike)
        {
            var npcID = reader.ReadInt16();

            Item selecteditem = player.SelectedItem;
            if (
                selecteditem.pick > 0
                || selecteditem.axe > 0
                || selecteditem.hammer > 0
                || selecteditem.damage <= 0
                || IsBoomerang(selecteditem.netID)
            )
                return;

            // If not using a boomerang
            player.SendData(PacketTypes.NpcUpdate, "", npcID);

            if (
                !LastInvalidWeapon.ContainsKey(player.Name)
                || (DateTime.Now - LastInvalidWeapon[player.Name]).Seconds >= 5
            )
            {
                LastInvalidWeapon[player.Name] = DateTime.Now;
                player.SendErrorMessage("You can only use a boomerang.");
            }
            args.Handled = true;
        }
    }
}
