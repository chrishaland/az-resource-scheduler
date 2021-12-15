import React, { useState } from 'react';
import { Badge, Button, Input, InputGroup, InputGroupText } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { cqrs } from '../cqrs.js';
import { useReactOidc } from "@axa-fr/react-oidc-context";
import CountdownTimer from "react-component-countdown-timer";

import './styles.css';
import './react-component-countdown-timer.scss'

export const ScheduleItem = (props) => {
    const { environment, environmentUptime } = props;
    const { oidcUser } = useReactOidc();

    const [uptime, setUptime] = useState(1);

    const onStartEnvironment = () => {
        cqrs('/api/schedule/start-environment-manually', { environmentId: environment.id, uptimeInMinutes: uptime * 60 }, oidcUser.id_token);
    };

    return (
        <div className="schedule-item col">
            {environment.description || environment.name}
            <Badge pill color={(environmentUptime?.resourcesCount ?? 0) > 0 ? "success" : "secondary"}>
                {environmentUptime?.resourcesCount ?? 0}
            </Badge>
            {environmentUptime ? (
                <CountdownTimer 
                    count={(new Date(environmentUptime?.scheduledStopTime) - new Date()) / 1000} 
                    size={11}
                    border
                    hideDay
                    direction="right"
                />
            ) : null}
            <InputGroup>
                <Input type="number" name="uptime" className="col-sm-1" value={uptime} onChange={event => setUptime(event.target.value)} />
                <InputGroupText>
                    <Locale id={"list-header-hours"} locales={locales}>hours</Locale>
                </InputGroupText>
                <Button color="info" onClick={() => onStartEnvironment(environment.id)}>
                    <Locale id={"list-header-start"} locales={locales}>Start</Locale>
                </Button>
            </InputGroup>
        </div>
    );
}
