import { Link, Route, Routes } from "react-router-dom";
import { AccountPage } from "./Auth/AccountPage";
import { LoginPage } from "./Auth/LoginPage";
import { LogoutButton } from "./Auth/LogoutButton";
import { BookListPage } from "./Books/BookListpage";
import { BookDetailsPage } from "./Books/BookDetailsPage";
import { RequireAdministrator } from "./Auth/RequireAdministrator";
import { CreateBookPage } from "./Books/CreateBookPage";
import { EditBookPage } from "./Books/EditBookPage";

function HomePage() {
  return <h1>Book Tracker</h1>;
}

export default function App() {
  return (
    <>
      <nav>
        <Link to="/">Home</Link> <Link to="/login">Log in</Link>{" "}
        <Link to="/account">Account</Link> <Link to="/books">Books</Link>{" "}
        <LogoutButton />
      </nav>
      <q></q>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/account" element={<AccountPage />} />
        <Route path="/books" element={<BookListPage />} />
        <Route element={<RequireAdministrator />}>
          <Route path="/books/new" element={<CreateBookPage />} />
          <Route path="/books/:bookId/edit" element={<EditBookPage />} />
        </Route>
        <Route path="/books/:bookId" element={<BookDetailsPage />} />
      </Routes>
    </>
  );
}
