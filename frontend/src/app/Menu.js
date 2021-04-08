import React, { useState } from 'react';
import { Collapse, Navbar, NavbarToggler, NavbarBrand, Nav } from 'reactstrap';
import { MenuItemAdmin } from './MenuItemAdmin';
import { MenuItemAccount } from './MenuItemAccount';
import { MenuItemLanguage } from './MenuItemLanguage';
import { locales } from './locales';
import { Locale } from '../translations/locale';
import './Menu.css';

export const Menu = () => {
    const [isOpen, setIsOpen] = useState(false);

    const toggle = () => setIsOpen(!isOpen);

    return (
        <div>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                <NavbarBrand href="/">
                    <Locale id={"header-title"} locales={locales}>Azure Resource Scheduler</Locale>
                </NavbarBrand>
                <NavbarToggler onClick={toggle} />
                <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={isOpen} navbar>
                    <Nav className="navbar-nav flex-grow" navbar>
                        <MenuItemAdmin />
                        <MenuItemLanguage />
                        <MenuItemAccount />
                    </Nav>
                </Collapse>
            </Navbar>
        </div>
    );
}
