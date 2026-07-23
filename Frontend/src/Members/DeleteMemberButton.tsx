import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { useCurrentMember } from "../Auth/UseCurrentMember";
import { deleteMember } from "./MembersApi";

type DeleteMemberButtonProps = {
  memberId: number;
  name: string;
};

export function DeleteMemberButton({ memberId, name }: DeleteMemberButtonProps) {
  const [confirming, setConfirming] = useState(false);
  const currentMemberQuery = useCurrentMember();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  function leaveDeletedMember() {
    queryClient.invalidateQueries({
      queryKey: ["members"],
      refetchType: "none",
    });
    queryClient.removeQueries({
      queryKey: ["books", "detail", memberId],
      exact: true,
    });
    navigate("/books");
  }

  const deleteMutation = useMutation({
    mutationFn: () => deleteMember(memberId),
    onSuccess: leaveDeletedMember,
  });

  if (
    !currentMemberQuery.isSuccess ||
    currentMemberQuery.data.role !== "Administrator"
  ) {
    return null;
  }

  if (!confirming) {
    return (
      <button type="button" onClick={() => setConfirming(true)}>
        Delete member
      </button>
    );
  }

    const mutationStatus =
    deleteMutation.error instanceof ApiError
      ? deleteMutation.error.status
      : null;

  return (
    <section aria-labelledby="delete-member-heading">
      <h2 id="delete-member-heading">Delete {name}?</h2>
      <p>This action cannot be undone.</p>

      <button
        type="button"
        onClick={() => deleteMutation.mutate()}
        disabled={deleteMutation.isPending}
      >
        {deleteMutation.isPending ? "Deleting..." : "Yes, delete member"}
      </button>{" "}

      <button
        type="button"
        onClick={() => {
          deleteMutation.reset();
          setConfirming(false);
        }}
        disabled={deleteMutation.isPending}
      >
        Cancel
      </button>

      {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
      {mutationStatus === 404 && (
        <div>
          <p>This member no longer exists. It may already have been deleted.</p>
          <button type="button" onClick={leaveDeletedMember}>
            Back to books
          </button>
        </div>
      )}
      {deleteMutation.isError && mutationStatus === null && (
        <p>Could not delete the member.</p>
      )}
    </section>
  );
}
