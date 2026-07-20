import { Link } from "react-router-dom";
import { useCurrentMember } from "../Auth/UseCurrentMember";

export function CreateBookLink() {
  const currentMemberQuery = useCurrentMember();

  if (
    !currentMemberQuery.isSuccess ||
    currentMemberQuery.data.role !== "Administrator"
  ) {
    return null;
  }

  return <Link to="/books/new">Add book</Link>;
}
