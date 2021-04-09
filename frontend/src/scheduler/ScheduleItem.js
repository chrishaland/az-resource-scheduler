import React, { useState } from 'react';
import { Button, Input, InputGroupAddon, InputGroupButtonDropdown, DropdownToggle, DropdownMenu, DropdownItem, InputGroup } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import './styles.css';
import { cqrs } from '../cqrs.js';

export const ScheduleItem = (props) => {
    const { environment } = props;
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const toggleDropDown = () => setDropdownOpen(!dropdownOpen);

    const [uptime, setUptime] = useState(60);

    const uptimes = {
        60: { id: "uptime-1h", default: "1 hour" },
        120: { id: "uptime-2h", default: "2 hours", value: 120 },
        240: { id: "uptime-4h", default: "4 hours", value: 240 },
        480: { id: "uptime-8h", default: "8 hours", value: 480 }
    };

    const onStartEnvironment = () => {
        cqrs('/api/schedule/start-environment-manually', { environmentId: environment.id, uptimeInMinutes: uptime });
    };

    return (
        <InputGroup>
            <Input placeholder={environment.description || environment.name} disabled />
            <InputGroupAddon addonType="append">
                <InputGroupButtonDropdown addonType="append" isOpen={dropdownOpen} toggle={toggleDropDown}>
                    <DropdownToggle split outline color="info">
                        <Locale id={uptimes[uptime].id} locales={locales}>{uptimes[uptime].default}</Locale>
                    </DropdownToggle>
                    <DropdownMenu>
                        {Object.keys(uptimes).map(key => (
                            <DropdownItem key={key} onClick={() => setUptime(key)}>
                                <Locale id={uptimes[key].id} locales={locales}>{uptimes[key].default}</Locale>
                            </DropdownItem>
                        ))}
                    </DropdownMenu>
                    <Button color="info" onClick={() => onStartEnvironment(environment.id)}>
                        <Locale id={"list-header-start"} locales={locales}>Start</Locale>
                    </Button>
                </InputGroupButtonDropdown>
            </InputGroupAddon>
        </InputGroup>
    );
}
