import React from 'react';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { useEnvironments } from '../environments/hooks';
import { ScheduleItem } from './ScheduleItem';
import './styles.css';

export const Scheduler = () => {
    const { environments } = useEnvironments();

    return (
        <div>
            <h1>
                <Locale id={"header"} locales={locales}>Start Environments</Locale>
            </h1>
            {environments.sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0)).map((environment, index) => (
                <ScheduleItem key={index} environment={environment} />
            ))}
        </div>
    );
}
