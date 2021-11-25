import React from 'react';
import { NavItem, NavLink, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import { Locale } from "../translations/locale";
import { locales } from "./locales";
import { useReactOidc } from "@axa-fr/react-oidc-context";
import { useOidcConfiguration } from '../oidc/store';

export const MenuItemAdmin = () => {
    const { oidcUser } = useReactOidc();
    const { oidcConfiguration } = useOidcConfiguration();

    return !!oidcUser.profile.roles && oidcUser.profile.roles.includes(oidcConfiguration.roles?.admin ?? "admin") ? (
        <UncontrolledDropdown nav inNavbar>
            <DropdownToggle nav caret>
                <Locale id={"header-admin-title"} locales={locales}>Admin</Locale>
            </DropdownToggle>
            <DropdownMenu>
                <DropdownItem>
                    <NavItem>
                        <NavLink tag={Link} to="/environments">
                            <Locale id={"header-admin-environments"} locales={locales}>Environments</Locale>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
                <DropdownItem>
                    <NavItem>
                        <NavLink tag={Link} to="/resources">
                            <Locale id={"header-admin-resources"} locales={locales}>Resources</Locale>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
                <DropdownItem>
                    <NavItem>
                        <NavLink tag={Link} to="/providers">
                            <Locale id={"header-admin-providers"} locales={locales}>Providers</Locale>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
                <DropdownItem divider />
                <DropdownItem>
                    <NavItem>
                        <NavLink href={"/jobs?token=" + oidcUser.id_token}>
                            <Locale id={"header-admin-jobs"} locales={locales}>Jobs</Locale>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
            </DropdownMenu>
        </UncontrolledDropdown>
    ) : null;
}
