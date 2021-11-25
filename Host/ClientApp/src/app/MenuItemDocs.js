import React from 'react';
import { NavItem, NavLink } from 'reactstrap';
import { Locale } from "../translations/locale";
import { locales } from "./locales";

export const MenuItemDocs = () => {
    return (
        <NavItem>
            <NavLink href="/docs">
                <Locale id={"header-admin-docs"} locales={locales}>Documentation</Locale>
            </NavLink>
        </NavItem>
    );
}
