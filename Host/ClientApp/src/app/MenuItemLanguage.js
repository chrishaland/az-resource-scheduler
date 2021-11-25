import React from 'react';
import { UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { useLanguage } from "../translations/store";
import { Locale } from "../translations/locale";
import { locales } from "./locales";

export const MenuItemLanguage = () => {
    const { state, dispatch } = useLanguage();

    const changeLanguage = language => {
        localStorage.setItem("language", language);
        dispatch({ type: "changeLanguage", language: language });
    };

    return (
        <UncontrolledDropdown>
            <DropdownToggle nav caret>
                <Locale id={"language-header"} locales={locales}>Language</Locale>
            </DropdownToggle>
            <DropdownMenu>
                <DropdownItem disabled={state.language === "no"} onClick={() => changeLanguage("no")}>
                    <Locale id={"language-no"} locales={locales}>Norwegian</Locale>
                </DropdownItem>
                <DropdownItem disabled={state.language === "en"} onClick={() => changeLanguage("en")}>
                    <Locale id={"language-en"} locales={locales}>English</Locale>
                </DropdownItem>
            </DropdownMenu>
        </UncontrolledDropdown>
    );
}
