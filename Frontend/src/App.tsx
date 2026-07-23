import { Route, Routes } from "react-router-dom";
import { AccountPage } from "./Auth/AccountPage";
import { LoginPage } from "./Auth/LoginPage";
import { BookListPage } from "./Books/BookListpage";
import { BookDetailsPage } from "./Books/BookDetailsPage";
import { RequireAdministrator } from "./Auth/RequireAdministrator";
import { CreateBookPage } from "./Books/CreateBookPage";
import { EditBookPage } from "./Books/EditBookPage";
import { RegisterPage } from "./Members/RegisterPage";
import { EditMemberPage } from "./Members/EditMemberPage";
import { MemberListPage } from "./Members/MemberListPage";
import { Navigation } from "./Navigation";


function HomePage() {
  return <h1>Book Tracker</h1>;
}

export default function App() {
  return (
    <>
      <Navigation />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/account" element={<AccountPage />} />
        <Route path="/books" element={<BookListPage />} />
        <Route path="/members/:memberId/edit" element={<EditMemberPage />} />
        <Route element={<RequireAdministrator />}>
          <Route path="/books/new" element={<CreateBookPage />} />
          <Route path="/books/:bookId/edit" element={<EditBookPage />} />
          <Route path="/members" element={<MemberListPage />} />
        </Route>
        <Route path="/books/:bookId" element={<BookDetailsPage />} />
        <Route path="/register" element={<RegisterPage />} />
      </Routes>
    </>
  );
}
