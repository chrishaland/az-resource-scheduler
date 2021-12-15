import React from 'react';
import { Table } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { useEnvironments } from '../environments/hooks';
import { useEnvironmentUptimes } from './hooks';
import { ScheduleItem } from './ScheduleItem';
import './styles.css';

export const Scheduler = () => {
    const { environments } = useEnvironments();
    const { environmentUptimes } = useEnvironmentUptimes();

    return !!environments && environments.length > 0 ? (
        <div>
            <h4>
                <Locale id={"header"} locales={locales}>Start Environments</Locale>
            </h4>
            <Table hover striped>
                <tbody>
                    {environments.sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0)).map((environment, index) => (
                        <tr key={index}>
                            <td scope="row">
                                <ScheduleItem environment={environment} environmentUptime={environmentUptimes[environment.id]} />
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </div>
    ) : null;
}
