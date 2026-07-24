import { Navigate } from "react-router-dom";
import { ApiError } from "../api";
import { getAccessToken } from "./tokenStorage";
import { useCurrentMember } from "./UseCurrentMember";
import { EditMemberLink } from "../Members/EditMemberLink";
import { DeleteMemberButton } from "../Members/DeleteMemberButton";

export function AccountPage() {
  const currentMemberQuery = useCurrentMember();

  if (!getAccessToken()) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.isPending) {
    return <p>Loading account...</p>;
  }

  const unauthorized =
    currentMemberQuery.error instanceof ApiError &&
    currentMemberQuery.error.status === 401;

  if (unauthorized) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.isError) {
    return <p>Could not load the account.</p>;
  }

  const member = currentMemberQuery.data;
  return (
    <main>

      <EditMemberLink memberId={member.id} ></EditMemberLink>{" "}
      <DeleteMemberButton memberId={member.id} name={member.name}></DeleteMemberButton>
      <h1>{member.name}</h1>
      <p>{member.email}</p>
      <p>Role: {member.role}</p>

    </main>
  );
}