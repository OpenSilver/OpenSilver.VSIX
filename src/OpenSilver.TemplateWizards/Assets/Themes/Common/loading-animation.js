function startLoader() {
    const count = document.querySelector(".odometer");
    const loader = document.querySelector(".loader-progress-bar");

    if (!count || !loader) return;

    loader.style.width = "0%";

    const observer = new MutationObserver(updateCount);
    function updateCount() {
        const loadPercentageText = getComputedStyle(document.documentElement)
            .getPropertyValue("--blazor-load-percentage-text")
            .trim();
        const loadPercentage = parseInt(loadPercentageText.replace(/"/g, ""));
        const currentValue = isNaN(loadPercentage) ? 0 : loadPercentage;

        animateCounter(currentValue);
        loader.style.width = currentValue + "%";

        if (currentValue === 100) {
            observer.disconnect();
            return;
        }
    }

    observer.observe(document.documentElement, {
        attributes: true,
        attributeFilter: ["style"],
    });

    updateCount();
}

function animateCounter(newValue) {
    newValue = Math.min(newValue, 100);

    const count = Array.from(document.querySelectorAll(".odometer"));
    const currentValue = count.map((span) => span.textContent).join("");
    const newValueString = String(newValue).padStart(3, "0");
    for (let i = 0; i < newValueString.length; i++) {
        if (newValueString[i] !== currentValue[i]) {
            gsap.to(count[i], {
                y: -4,
                opacity: 0.5,
                duration: 0.075,
                ease: "none",
                onComplete: () => {
                    count[i].textContent = newValueString[i];
                    gsap.fromTo(
                        count[i],
                        { y: 4, opacity: 0.5 },
                        { y: 0, opacity: 1, duration: 0.075, ease: "none" }
                    );
                },
            });
        }
    }

    if (newValue === 100 && currentValue !== "100") {
        gsap.to(count[1], {
            y: -4,
            opacity: 0.5,
            duration: 0.075,
            ease: "none",
            onComplete: () => {
                count[1].textContent = "0";
                gsap.fromTo(
                    count[1],
                    { y: 4, opacity: 0.5 },
                    { y: 0, opacity: 1, duration: 0.075, ease: "none" }
                );
            },
        });

        gsap.to(count[2], {
            y: -4,
            opacity: 0.5,
            duration: 0.075,
            ease: "none",
            onComplete: () => {
                count[2].textContent = "0";
                gsap.fromTo(
                    count[2],
                    { y: 4, opacity: 0.5 },
                    { y: 0, opacity: 1, duration: 0.075, ease: "none" }
                );
            },
        });
    }
}

document.addEventListener('DOMContentLoaded', function () {
    startLoader();

    const loadingLogo = document.querySelector(".loading-logo");

    function startAnimations() {
        gsap.to(".loading-logo", {
            width: "100px",
            scale: 1,
            duration: 0.4,
            ease: "elastic",
        });

        gsap.to(".loader-progress", {
            width: "60vw",
            opacity: 1,
            duration: 1.25,
            ease: "power1.out",
            delay: 0.4,
        });

        gsap.to(".counter-container", {
            opacity: 1,
            duration: 0.3,
            ease: "none",
            delay: 1.1,
        });

        gsap.to(".counter-container > .odometer", {
            transform: "translateY(0)",
            duration: 0.3,
            ease: "none",
            delay: 1.1,
        });
    }

    if (loadingLogo == null || loadingLogo.complete) {
        startAnimations();
    } else {
        loadingLogo.addEventListener("load", startAnimations);
    }
});
