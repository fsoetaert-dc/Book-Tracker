import { Link, Route, Routes } from "react-router-dom";
import { AccountPage } from "./Auth/AccountPage";
import { LoginPage } from "./Auth/LoginPage";
import { LogoutButton } from "./Auth/LogoutButton";

function HomePage() {
  return <h1>Book Tracker</h1>;
}

export default function App() {
  return (
    <>
      <nav>
        <Link to="/">Home</Link>{" "}
        <Link to="/login">Log in</Link>{" "}
        <Link to="/account">Account</Link>{" "}
        <LogoutButton />
      </nav>

      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/account" element={<AccountPage />} />
      </Routes>
    </>
  );
}