using System;
using GTA;
using iFruitAddon2;
using MMI_SP.Debug;

namespace MMI_SP.iFruit
{
    public class Core : Script
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private readonly CustomiFruit _iFruit;
        private Menu _menu;
        private ConfigMenu _configMenu;
        private Action _menuClosedUnsubscribe;

        // ==========================================
        // BLOQUE 2: Constructor y eventos
        // ==========================================
        public Core()
        {
            _iFruit = new CustomiFruit();
            Tick += Initialize;
            Aborted += OnAborted;
        }

        private void Initialize(object sender, EventArgs e)
        {
            while (!Insurance.Observer.Manager.Initialized) Yield();

            _menu = new Menu();
            _configMenu = new ConfigMenu();

            Wait(2000);

            var contactMMI = new iFruitContact("Mors Mutual Insurance")
            {
                DialTimeout = 4000,
                Active = true,
                Icon = ContactIcon.MP_MorsMutual
            };
            contactMMI.Answered += OnMMIAnswered;

            var contactConfig = new iFruitContact("Configuración MMI")
            {
                DialTimeout = 0,
                Active = true,
                Icon = ContactIcon.MP_FmContact
            };
            contactConfig.Answered += OnConfigAnswered;

            _iFruit.Contacts.Add(contactMMI);
            _iFruit.Contacts.Add(contactConfig);

            Tick -= Initialize;
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            _menu?.MenuPoolProcessMenus();
            _configMenu?.MenuPoolProcessMenus();

            if (_menu != null && _menu.IsVisible)
            {
                Game.DisableControlThisFrame(Control.PhoneUp);
                Game.DisableControlThisFrame(Control.PhoneDown);
                Game.DisableControlThisFrame(Control.PhoneLeft);
                Game.DisableControlThisFrame(Control.PhoneRight);
                Game.DisableControlThisFrame(Control.PhoneSelect);
                Game.DisableControlThisFrame(Control.PhoneCancel);
                Game.DisableControlThisFrame(Control.PhoneOption);
            }

            _iFruit.Update();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            if (_iFruit?.Contacts?.Count > 0)
                _iFruit.Contacts.ForEach(c => c.EndCall());
        }

        private void OnMMIAnswered(iFruitContact contact)
        {
            MMISound.Play(MMISound.SoundFamily.Hello);
            _menu.Reset(true);
            _menu.Show();
            _menuClosedUnsubscribe?.Invoke();
            _menuClosedUnsubscribe = _menu.OnMainMenuClosed(OnMenuClosed);
        }

        private void OnConfigAnswered(iFruitContact contact)
        {
            _configMenu.Show();
            _iFruit.Close(5000);
        }

        private void OnMenuClosed()
        {
            MMISound.Play(MMISound.SoundFamily.Bye);
            _menuClosedUnsubscribe?.Invoke();
            _menuClosedUnsubscribe = null;
        }
    }
}