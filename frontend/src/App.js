import React from 'react';
import { Route } from 'react-router';
import { Layout } from './app/Layout';
import { Scheduler } from './scheduler/Scheduler';
import { Environments } from './environments/Environments';
import { Tenants } from './tenants/Tenants';
import { LanguageProvider } from './translations/store';
import { TenantsProvider } from './tenants/store';
import { EnvironmentsProvider } from './environments/store';

export const App = () => {
    return (
        <LanguageProvider>
            <TenantsProvider>
                <EnvironmentsProvider>
                    <Layout>
                        <Route exact path='/' component={Scheduler} />
                        <Route path='/tenants' component={Tenants} />
                        <Route path='/environments' component={Environments} />
                    </Layout>
                </EnvironmentsProvider>
            </TenantsProvider>
        </LanguageProvider>
    );
}

export default App;