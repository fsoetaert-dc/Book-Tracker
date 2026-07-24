import { Link } from "react-router-dom";
import { useCurrentMember } from "../Auth/UseCurrentMember";

type EditMemberLinkProps = {
    memberId: number;
};

export function EditMemberLink({ memberId }: EditMemberLinkProps) {
    const currentMemberQuery = useCurrentMember();

    if (
        !currentMemberQuery.isSuccess) {
        return null;
    }

    return <Link to={`/members/${memberId}/edit`}>Edit Account</Link>;
}