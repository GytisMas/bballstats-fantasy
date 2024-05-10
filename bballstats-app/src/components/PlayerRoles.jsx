export default function PlayerRoles(id) {
    if (id >= 0 && id < 5)
        return roles[id];
    else
        return 0;
}
export const roles = ["Guard", "Forward", "Center", "Point Guard", "Shooting Guard", "Small forward", "Power forward", "Center"];