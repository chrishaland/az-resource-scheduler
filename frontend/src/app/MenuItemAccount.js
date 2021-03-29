import React from 'react';
import { NavItem, NavLink } from 'reactstrap';
import { Resource } from "../translations/resource";
import { resources } from "./resources";

export const MenuItemAccount = () => {
    const isLoggedIn = () => {
        return false;
    };

    return (
        <NavItem>
            {isLoggedIn() ? (
                <NavLink href="/api/account/logout">
                    <Resource id={"header-admin-logout"} resources={resources}>Log out</Resource>
                </NavLink>
            ) : (
                <NavLink href="/api/account/login">
                    <Resource id={"header-admin-login"} resources={resources}>Log in</Resource>
                </NavLink>
            )}
        </NavItem>
    );
}
